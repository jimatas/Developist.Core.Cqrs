namespace Developist.Core.Cqrs.Tests.Fixture;

public class EmptyHandlerRegistry : IHandlerRegistry
{
    public ICommandHandler<TCommand> GetCommandHandler<TCommand>()
        where TCommand : ICommand
    {
        return null!;
    }

    public IOrderedEnumerable<ICommandInterceptor<TCommand>> GetCommandInterceptors<TCommand>()
        where TCommand : ICommand
    {
        return Array.Empty<ICommandInterceptor<TCommand>>().OrderBy(_ => PriorityLevel.Normal);
    }

    public IEnumerable<IEventHandler<TEvent>> GetEventHandlers<TEvent>()
        where TEvent : IEvent
    {
        return Array.Empty<IEventHandler<TEvent>>();
    }

    public IQueryHandler<TQuery, TResult> GetQueryHandler<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        return null!;
    }

    public IOrderedEnumerable<IQueryInterceptor<TQuery, TResult>> GetQueryInterceptors<TQuery, TResult>()
        where TQuery : IQuery<TResult>
    {
        return Array.Empty<IQueryInterceptor<TQuery, TResult>>().OrderBy(_ => PriorityLevel.Normal);
    }
}
