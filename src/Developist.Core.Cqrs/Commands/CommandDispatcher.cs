using Developist.Core.Cqrs.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Commands
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ICommandDispatcher"/> interface.
    /// This class is sealed and cannot be inherited.
    /// </summary>
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IHandlerRegistry _registry;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcher"/> class with the specified handler registry and optional logger.
        /// </summary>
        /// <param name="registry">The handler registry that the dispatcher will use to look up message handlers.</param>
        /// <param name="logger">An optional logger instance that the dispatcher will use for logging.
        /// If not provided, a <see cref="NullLogger"/> instance will be used.</param>
        public CommandDispatcher(IHandlerRegistry registry, ILogger<CommandDispatcher> logger = default)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _logger = logger ?? NullLogger<CommandDispatcher>.Instance;
        }

        /// <inheritdoc/>
        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            if ((object)command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = _registry.GetCommandHandler<TCommand>();
            var interceptors = _registry.GetCommandInterceptors<TCommand>();

            try
            {
                await ExecutePipeline().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Unhandled exception during command dispatch: {ExceptionMessage}", exception.Message);
                throw;
            }

            Task ExecutePipeline()
            {
                CommandHandlerDelegate<TCommand> pipeline = (c, ct) => handler.HandleAsync(c, ct);
                foreach (var interceptor in interceptors)
                {
                    pipeline = Pipe(pipeline, interceptor);
                }
                return pipeline(command, cancellationToken);
            }

            CommandHandlerDelegate<TCommand> Pipe(CommandHandlerDelegate<TCommand> next, ICommandInterceptor<TCommand> interceptor)
            {
                return (c, ct) => interceptor.InterceptAsync(c, next, ct);
            }
        }
    }
}
