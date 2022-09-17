using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class CommandWithMultipleHandlers : ICommand
    {
    }

    public class CommandWithMultipleHandlersFirstHandler : ICommandHandler<CommandWithMultipleHandlers>
    {
        public Task HandleAsync(CommandWithMultipleHandlers command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class CommandWithMultipleHandlersSecondHandler : ICommandHandler<CommandWithMultipleHandlers>
    {
        public Task HandleAsync(CommandWithMultipleHandlers command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
