using System;
using System.Linq.Expressions;

namespace Developist.Core.Cqrs
{
    internal static class ArgumentNullExceptionHelper
    {
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
