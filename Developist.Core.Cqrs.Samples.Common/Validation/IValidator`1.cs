// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs.Samples.Common.Validation
{
    /// <summary>
    /// Defines the interface for a class that validates commands.
    /// </summary>
    /// <typeparam name="TCommand">The type of command validated.</typeparam>
    public interface IValidator<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Ensures the specified command is valid by throwing an exception when it is not.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <exception cref="ValidationFailedException">If the command does not validate.</exception>
        void Validate(TCommand command)
        {
            if (!TryValidate(command, out var errors))
            {
                throw new ValidationFailedException($"Validation failed for command {command.GetType().Name}. "
                    + $"See the individual validation errors in the {nameof(ValidationFailedException)}.{nameof(ValidationFailedException.Errors)} property for details.", errors);
            }
        }

        /// <summary>
        /// Validates the specified command without throwing an exception.
        /// Instead, collects the validation errors and returns them via the out parameter.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <param name="errors">The validation error(s) to return.</param>
        /// <returns><see langword="true"/> or <see langword="false"/>, depending on whether the command is valid or not.
        /// If <see langword="false"/>, the out parameter is expected to contain at least one validation error.</returns>
        bool TryValidate(TCommand command, out ValidationError[] errors);
    }
}
