// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Cqrs.Samples.Common.Validation
{
    /// <summary>
    /// Describes a validation error with a message and optional, positional, format parameters to interpolate into that message.
    /// </summary>
    public record ValidationError(string Message, params object[] MessageParameters)
    {
        /// <inheritdoc/>
        public override string ToString() => string.Format(Message, MessageParameters);
    }
}
