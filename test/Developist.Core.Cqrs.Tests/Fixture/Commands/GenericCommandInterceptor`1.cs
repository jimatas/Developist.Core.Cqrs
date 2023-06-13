namespace Developist.Core.Cqrs.Tests.Fixture.Commands;

public class GenericCommandInterceptor<TCommand> : ICommandInterceptor<TCommand>
    where TCommand : ICommand
{
    public Task InterceptAsync(TCommand command, CommandHandlerDelegate<TCommand> next, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
