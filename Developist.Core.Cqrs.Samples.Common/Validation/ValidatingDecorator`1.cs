// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Samples.Common.Validation
{
    /// <summary>
    /// Adds validation behavior to the command processing pipeline.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to validate.</typeparam>
    public class ValidatingDecorator<TCommand> : ICommandHandlerWrapper<TCommand> where TCommand : ICommand
    {
        private readonly ILogger<ValidatingDecorator<TCommand>> logger;
        private readonly IValidator<TCommand> validator;

        public ValidatingDecorator(ILogger<ValidatingDecorator<TCommand>> logger, IValidator<TCommand> validator = null)
        {
            this.logger = logger ?? NullLogger<ValidatingDecorator<TCommand>>.Instance;
            this.validator = validator;
        }

        public async Task HandleAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken)
        {
            if (validator is null)
            {
                logger.LogDebug("No validator configured for command {CommandType}.", command.GetType().Name);
            }
            else
            {
                logger.LogDebug("Validating command {CommandType}.", command.GetType().Name);
                validator.Validate(command);
                logger.LogDebug("Successfully validated command {CommandType}.", command.GetType().Name);
            }

            await next();
        }
    }
}
