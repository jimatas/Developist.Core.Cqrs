# Developist.Core.Cqrs
Splits the application's use-case processing layer into components that fit into one of two categories: _commands_ and _queries_. This library also adds the closely linked _events_ to these two categories.

Define your commands by inheriting from the `ICommand` interface; your queries by inheriting from the `IQuery<TResult>` interface; and your events by inheriting from the `IEvent` interface. These marker interfaces are meant to enable the automatic wire-up of messages with handlers by the dispatcher.

Each type of message is handled by a class that implements that message's respective handler interface. That is, `ICommandHandler<TCommand>` for commands, `IQueryHandler<TQuery, TResult>` for queries, and `IEventHandler<TEvent>` for events. A command and a query will have exactly one handler associated with them, whereas an event can be handled by multiple handlers, or none at all.

Furthermore, command and query handlers can be wrapped by _decorators_ to create processing pipelines for easy extensibility. For this purpose, the `ICommandHandlerWrapper<TCommand>` and `IQueryHandlerWrapper<TQuery, TResult>` interfaces are provided.

Other than calling the `AddCqrs` extension method on `IServiceCollection`, no additional registration is required for the dispatcher to be able to resolve your handlers or wrappers at runtime.
