// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Cqrs.Samples.Common.Validation
{
    /// <summary>
    /// The exception that is thrown to indicate that a command did not validate.
    /// </summary>
    public class ValidationFailedException : Exception
    {
        #region Constructors
        public ValidationFailedException() : this(errors: null) { }
        public ValidationFailedException(ValidationError[] errors) : this(message: null, errors) { }
        public ValidationFailedException(string message, ValidationError[] errors) : base(message) => Errors = errors ?? Array.Empty<ValidationError>();
        #endregion

        /// <summary>
        /// A possibly empty array of <see cref="ValidationError"/> objects describing the reason(s) for validation failure.
        /// </summary>
        public ValidationError[] Errors { get; }
    }
}
