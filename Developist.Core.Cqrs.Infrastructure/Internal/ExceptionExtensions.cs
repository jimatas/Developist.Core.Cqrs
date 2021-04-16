// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Text;

namespace Developist.Core.Cqrs
{
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Returns a detailed description of the exception that occurred.
        /// </summary>
        /// <param name="exception">The exception for which to return a detail message.</param>
        /// <param name="includeInnerExceptions">If <see langword="true"/>, will include the details of any inner exceptions.</param>
        /// <returns>A string detailing the exception, including its stack trace information.</returns>
        public static string DetailMessage(this Exception exception, bool includeInnerExceptions = true)
        {
            var detailMessageBuilder = new StringBuilder(exception.BuildDetailMessage());

            if (includeInnerExceptions)
            {
                exception.AppendInnerExceptionsTo(detailMessageBuilder);
            }

            return detailMessageBuilder.ToString();
        }

        private static string BuildDetailMessage(this Exception exception)
        {
            var detailMessage = $"{exception.GetType().Name}: {exception.Message}";

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                detailMessage += Environment.NewLine + exception.StackTrace;
            }

            return detailMessage;
        }

        private static void AppendInnerExceptionsTo(this Exception exception, StringBuilder detailMessageBuilder, int depth = 0)
        {
            if (exception.InnerException is not null)
            {
                depth++;
                detailMessageBuilder.Append($" [{nameof(exception.InnerException)} ({depth}): {exception.InnerException.BuildDetailMessage()}]");

                exception.InnerException.AppendInnerExceptionsTo(detailMessageBuilder, depth);
            }
        }
    }
}
