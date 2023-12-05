# Developist.Core.Cqrs

### Lightweight (no dependency) in-process CQRS library

To define a command message, simply inherit from the [`ICommand`](src/Developist.Core.Cqrs/Commands/ICommand.cs) interface.
Similarly, to define a query message, inherit from the [`IQuery<TResult>`](src/Developist.Core.Cqrs/Queries/IQuery`1.cs) generic interface.
For notification-style messages, inherit from the [`IEvent`](src/Developist.Core.Cqrs/Events/IEvent.cs) interface.
Note that these are marker interfaces and do not declare any members that have to be implemented.

#### Handling
To process instances of these message types, you need to define handlers by inheriting from the appropriate interfaces.
Specifically, you should derive from the [`ICommandHandler<T>`](src/Developist.Core.Cqrs/Commands/ICommandHandler`1.cs), [`IQueryHandler<T, TResult>`](src/Developist.Core.Cqrs/Queries/IQueryHandler`2.cs), or [`IEventHandler<T>`](src/Developist.Core.Cqrs/Events/IEventHandler`1.cs) generic interfaces.
All of these interfaces declare a single `HandleAsync` method that needs to be implemented.

For example, to handle the `SendEmailCommand` message, you need to create a handler that derives from `ICommandHandler<SendEmailCommand>` and implements the `HandleAsync(SendEmailCommand command, CancellationToken cancellationToken)` method.

The main difference between a command and a query is that the latter returns a result while the former does not.
Additionally, each command and query can only be processed by a single handler.
If you define more than one or no handler at all for a particular command or query, the dispatcher will throw an exception.
In contrast, a notification can be processed by zero, one, or more than one handler.

#### Dispatching
The routing of messages to handlers is done by the dispatcher, which is a type that implements the [`IDispatcher`](src/Developist.Core.Cqrs/IDispatcher.cs) interface.
Note that this top-level interface simply combines the individual [`ICommandDispatcher`](src/Developist.Core.Cqrs/Commands/ICommandDispatcher.cs), [`IQueryDispatcher`](src/Developist.Core.Cqrs/Queries/IQueryDispatcher.cs) and [`IEventDispatcher`](src/Developist.Core.Cqrs/Events/IEventDispatcher.cs) interfaces into a single convenient interface.

The library provides default implementations for all dispatcher interfaces.
These implementations use the built-in dependency injection framework to resolve handlers at runtime.
Moreover, the library provides registration extensions for DI, specifically the `CqrsBuilderExtensions.AddHandlersFromAssembly` method, which automatically scans for handlers and maps them to their corresponding messages based on generic type information.
This eliminates the need for manual registration of handlers.

#### Intercepting
Another concept included in this library is that of interceptors.
Interceptors can be thought of as filters that allow you to add additional behavior around message processing.
Their main purpose is to modify or enhance the behavior of a command or query handler.

When a message is dispatched, the dispatcher arranges all the registered interceptors for that message type into a pipeline. Each interceptor in the pipeline has the ability to modify the message or the result (in the case of a query) and is responsible for invoking the next interceptor using the provided delegate function. By choosing not to invoke the delegate, the interceptors possess the ability to effectively cancel the processing of a message entirely.

The pipeline starts with the first interceptor and continues in order until it reaches the last interceptor, and ultimately, the handler.
To ensure that interceptors run in a predetermined order, they can be annotated with the [`PipelinePriorityAttribute`](src/Developist.Core.Cqrs/PipelinePriorityAttribute.cs). This attribute allows you to assign a priority level to each interceptor, indicating the order in which they should be executed.
