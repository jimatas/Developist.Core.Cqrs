using Developist.Core.Cqrs.Commands;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public class SampleCommand : ICommand
    {
    }

    public class SampleCommandHandler : ICommandHandler<SampleCommand>
    {
        private readonly Queue<Type> log;
        public SampleCommandHandler(Queue<Type> log) => this.log = log;
        public Task HandleAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            log.Enqueue(GetType());
            return Task.CompletedTask;
        }
    }
}
