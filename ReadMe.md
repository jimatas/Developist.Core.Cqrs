# Developist.Core.Cqrs
Split an application's architecture into two distinct categories or roles: _commands_ and _queries_. This library also adds _events_ to these two categories.

Define your commands by inheriting from the `ICommand` interface; your queries by inheriting from the `IQuery<TResult>` interface; and your events by inheriting from the `IEvent` interface. These marker interfaces are meant to enable the automatic wire-up of messages with handlers by the dispatcher.

Each type of message is handled by a class that implements that message's respective handler interface. That is, `ICommandHandler<TCommand>` for commands, `IQueryHandler<TQuery, TResult>` for queries, and `IEventHandler<TEvent>` for events. A command and a query will have exactly one handler associated with them, whereas an event can be handled by multiple handlers, or none at all.

Additionally, command and query handlers can be wrapped by _decorators_ to create processing pipelines for easy extensibility. For this purpose, the `ICommandHandlerWrapper<TCommand>` and `IQueryHandlerWrapper<TQuery, TResult>` interfaces are provided.
