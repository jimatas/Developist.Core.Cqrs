// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Samples.Common.Diagnostics
{
    public class DiagnosticDecorator<TQuery, TResult> : IQueryHandlerWrapper<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly ILogger logger;
        public DiagnosticDecorator(ILogger<DiagnosticDecorator<TQuery, TResult>> logger) => this.logger = logger;

        public async Task<TResult> HandleAsync(TQuery query, HandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            logger.LogDebug("Executing query {QueryType} {QueryDetails}", query.GetType().Name, SerializeDeferred(query));

            var stopwatch = Stopwatch.StartNew();
            var result = await next();
            stopwatch.Stop();

            logger.LogDebug("Executed query {QueryType} in {Milliseconds} ms.", query.GetType().Name, stopwatch.ElapsedMilliseconds);

            return result;
        }

        private static object SerializeDeferred(object value)
        {
            return new DeferredToString(() => JsonSerializer.Serialize(value, CreateDefaultSerializerOptions()));

            static JsonSerializerOptions CreateDefaultSerializerOptions() => new()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }
    }
}
