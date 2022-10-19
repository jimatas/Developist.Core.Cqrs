# Developist.Core.Cqrs

### Lightweight (no-dependency) and low-ceremony CQRS library

Define your command messages by inheriting from the [`ICommand`](Developist.Core.Cqrs/Commands/ICommand.cs) interface and your query messages by inheriting from the [`IQuery<TResult>`](Developist.Core.Cqrs/Queries/IQuery`1.cs) generic interface. For notification style messages you can inherit from the [`IEvent`](Developist.Core.Cqrs/Events/IEvent.cs) interface. Note that these are all marker interfaces, they do not declare any members that have to be implemented.

#### Handling
Instances of these message types are processed by handlers that you define by inheriting from respectively the [`ICommandHandler<T>`](Developist.Core.Cqrs/Commands/ICommandHandler`1.cs), [`IQueryHandler<T, TResult>`](Developist.Core.Cqrs/Queries/IQueryHandler`2.cs) and [`IEventHandler<T>`](Developist.Core.Cqrs/Events/IEventHandler`1.cs) generic interfaces. These interfaces all declare a single `HandleAsync` method that will have to be implemented.

For example, the `SendEmailCommand` message, which is a command, is processed by a handler that derives from `ICommandHandler<SendEmailCommmand>` and implements the `HandleAsync(SendEmailCommand command, CancellationToken cancellationToken)` method.

The difference between a command and a query is that the latter returns a result while the former does not. Both of these message types can also only be processed by a single handler. Defining more than one, or no handler at all for a particular command or query will cause an exception to be thrown by the dispatcher. A notification on the other hand can be processed by zero, one, or more than one handler.

#### Dispatching
Routing messages to handlers is accomplished by the aforementioned dispatcher, which is a type that implements the [`IDispatcher`](Developist.Core.Cqrs/IDispatcher.cs) interface. Note that this top-level interface simply combines the individual [`ICommandDispatcher`](Developist.Core.Cqrs/Commands/ICommandDispatcher.cs), [`IQueryDispatcher`](Developist.Core.Cqrs/Queries/IQueryDispatcher.cs) and [`IEventDispatcher`](Developist.Core.Cqrs/Events/IEventDispatcher.cs) interfaces into a single convenient interface. Likewise, the [`IDynamicDispatcher`](Developist.Core.Cqrs/IDynamicDispatcher.cs) does the same for the [`IDynamicCommandDispatcher`](Developist.Core.Cqrs/Commands/IDynamicCommandDispatcher.cs), [`IDynamicQueryDispatcher`](Developist.Core.Cqrs/Queries/IDynamicQueryDispatcher.cs) and [`IDynamicEventDispatcher`](Developist.Core.Cqrs/Events/IDynamicEventDispatcher.cs) interfaces.

The difference between regular (static) and dynamic dispatch is that the latter supports polymorphic dispatch of messages through the use of some reflection. 
The best way to show what this means is by an example.

```csharp
ICommand command = new SendEmailCommand(to, from, subject, body); // Note, the SendEmailCommand is assigned to a variable of type ICommand.
await dispatcher.DispatchAsync(command); // Fails with "No handler found for command with type 'Developist.Core.Cqrs.Commands.ICommand'."
await dynamicDispatcher.DispatchAsync(command) // Routes the ICommand parameter successfully to a handler that processes SendEmailCommand messages.
```

Default implementations for both dispatcher interfaces are provided by the library. Internally, these implementations use the built-in dependency injection framework to resolve handlers at runtime. Using the DI registration extensions, specifically the `CqrsBuilderExtensions.AddHandlersFromAssembly` method, there will also be no need to manually register any of your handlers as they will be picked up and mapped to your messages automatically based on the available generic type information.

#### Intercepting
Another concept included in this library is that of interceptors. An interceptor is very much like a wrapper or decorator, and is mainly used to dynamically add addtional behavior around message processing. Multiple interceptors can be defined for a message type, or indeed entire sets of message types by utilizing open generic type parameters. These interceptors will be arranged back-to-back by the dispatcher, and as such will comprise a processing pipeline together with the handler.

Both command and query dispatch support message interception. For this purpose the [`ICommandInterceptor<T>`](Developist.Core.Cqrs/Commands/ICommandInterceptor`1.cs) and [`IQueryInterceptor<T, TResult>`](Developist.Core.Cqrs/Queries/IQueryInterceptor`2.cs) interfaces are provided by the library. Depending on which message type you wish to intercept, you derive from the appropriate interface and implement the single `InterceptAsync` method. This method gets passed in the message as well as a function delegate that represents the call to the next interceptor, or ultimately the handler. So by not invoking this delegate you effectively shortcircuit the pipeline, stopping further processing.

Just like handlers, interceptors will also be discovered automatically and wired-up with the appropriate message types by the dispatcher. If your interceptors need to run in some predetermined order, you can override the `Priority` property, which defaults to `PriorityLevel.Normal`. The dispatcher will order interceptors by taking into account their declared priority, with a higher priority meaning the interceptor runs earlier in the pipeline.
