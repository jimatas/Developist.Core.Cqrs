using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Developist.Core.Cqrs
{
    internal static class ArgumentNullExceptionHelper
    {
        [return: NotNull]
        public static T ThrowIfNull<T>(Expression<Func<T>> argument)
        {
            var value = argument.Compile().Invoke();
            if (value is null)
            {
                var paramName = (argument.Body as MemberExpression ?? (argument.Body as UnaryExpression)?.Operand as MemberExpression)?.Member.Name;
                throw new ArgumentNullException(paramName);
            }
            return value;
        }
    }
}
