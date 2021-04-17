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
    public class DiagnosticDecorator<TCommand> : ICommandHandlerWrapper<TCommand> where TCommand : ICommand
    {
        private readonly ILogger logger;
        public DiagnosticDecorator(ILogger<DiagnosticDecorator<TCommand>> logger) => this.logger = logger;

        public async Task HandleAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            logger.LogDebug("Executing command {CommandType} {CommandDetails}", command.GetType().Name, SerializeDeferred(command));

            var stopwatch = Stopwatch.StartNew();
            await next();
            stopwatch.Stop();

            logger.LogDebug("Executed command {CommandType} in {Milliseconds} ms.", command.GetType().Name, stopwatch.ElapsedMilliseconds);
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
