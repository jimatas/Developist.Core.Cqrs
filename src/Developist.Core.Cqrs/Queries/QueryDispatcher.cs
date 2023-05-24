using Developist.Core.Cqrs.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Queries
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IQueryDispatcher"/> interface.
    /// This class is sealed and cannot be inherited.
    /// </summary>
    public sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly ConcurrentDictionary<(Type, Type), Delegate> _dispatcherDelegates = new ConcurrentDictionary<(Type, Type), Delegate>();
        private readonly IHandlerRegistry _registry;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcher"/> class with the specified handler registry and optional logger.
        /// </summary>
        /// <param name="registry">The handler registry that the dispatcher will use to look up message handlers.</param>
        /// <param name="logger">An optional logger instance that the dispatcher will use for logging.
        /// If not provided, a <see cref="NullLogger"/> instance will be used.</param>
        public QueryDispatcher(IHandlerRegistry registry, ILogger<QueryDispatcher> logger = default)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _logger = logger ?? NullLogger<QueryDispatcher>.Instance;
        }

        /// <inheritdoc/>
        public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var queryType = query.GetType();
            var resultType = typeof(TResult);

            var dispatcherDelegate = (QueryDispatcherDelegate<TResult>)_dispatcherDelegates
                .GetOrAdd((queryType, resultType), _ => CreateDispatcherDelegate());

            return dispatcherDelegate.Invoke(query, cancellationToken);

            Delegate CreateDispatcherDelegate()
            {
                var dispatchMethod = typeof(QueryDispatcher).GetMethods()
                    .Single(method => method.Name == nameof(DispatchAsync) && method.GetGenericArguments().Length == 2)
                    .MakeGenericMethod(queryType, resultType);

                return Delegate.CreateDelegate(typeof(QueryDispatcherDelegate<TResult>), this, dispatchMethod);
            }
        }

        /// <summary>
        /// Dispatches a query of type <typeparamref name="TQuery"/> asynchronously to its registered handler and returns the result.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query to be dispatched, which must implement the <see cref="IQuery{TResult}"/> interface.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
        /// <param name="query">The query to be dispatched.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, which returns the result of the query.</returns>
        public async Task<TResult> DispatchAsync<TQuery, TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handler = _registry.GetQueryHandler<TQuery, TResult>();
            var interceptors = _registry.GetQueryInterceptors<TQuery, TResult>();

            try
            {
                return await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Unhandled exception during query dispatch: {ExceptionMessage}", exception.Message);

                throw;
            }

            Task<TResult> ExecutePipeline()
            {
                QueryHandlerDelegate<TQuery, TResult> pipeline = (q, ct) => handler.HandleAsync(q, ct);
                foreach (var interceptor in interceptors)
                {
                    pipeline = Pipe(pipeline, interceptor);
                }

                return pipeline((TQuery)query, cancellationToken);
            }

            QueryHandlerDelegate<TQuery, TResult> Pipe(QueryHandlerDelegate<TQuery, TResult> next, IQueryInterceptor<TQuery, TResult> interceptor)
            {
                return (q, ct) => interceptor.InterceptAsync(q, next, ct);
            }
        }
    }
}
