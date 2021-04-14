// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs
{
    public interface ICommandHandlerWrapper<in TCommand> : ISortable where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, HandlerDelegate next, CancellationToken cancellationToken);
    }
}
