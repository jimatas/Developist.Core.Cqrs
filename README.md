# Developist.Core.Cqrs

A lightweight in-process CQRS library simplifying command, query, and event handling.

## Messages and Handlers
Define messages using simple interfaces:

- [`ICommand`](./src/Developist.Core.Cqrs/Commands/ICommand.cs): For command messages.
- [`IQuery<TResult>`](./src/Developist.Core.Cqrs/Queries/IQuery.cs): For query messages.
- [`IEvent`](./src/Developist.Core.Cqrs/Events/IEvent.cs): For notification-style messages.

Create message handlers by implementing corresponding interfaces:

- [`ICommandHandler<T>`](./src/Developist.Core.Cqrs/Commands/ICommandHandler.cs): For handling commands.
- [`IQueryHandler<T, TResult>`](./src/Developist.Core.Cqrs/Queries/IQueryHandler.cs): For handling queries.
- [`IEventHandler<T>`](./src/Developist.Core.Cqrs/Events/IEventHandler.cs): For handling events.

## Dispatching
Easily dispatch messages to handlers:

- Use the [dispatcher](./src/Developist.Core.Cqrs/DefaultDispatcher.cs) to route messages to their respective handlers.
- [Automatic handler registration](./src/Developist.Core.Cqrs/DependencyInjection/CqrsConfiguratorExtensions.cs) through dependency injection.
- No manual handler registration needed.

## Interception
Enhance message processing with interceptors:

- Modify or enhance [command](./src/Developist.Core.Cqrs/Commands/ICommandInterceptor.cs)/[query](./src/Developist.Core.Cqrs/Queries/IQueryInterceptor.cs) behavior by forming a customizable pipeline with execution order.
- Use the [`PipelinePriorityAttribute`](./src/Developist.Core.Cqrs/PipelinePriorityAttribute.cs) to specify interceptor order.