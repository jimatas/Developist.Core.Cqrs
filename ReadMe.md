# Developist.Core.Cqrs
Aids in splitting the application's use-case or application layer into components that fit into one of two categories: _commands_ or _queries_. This library also includes support for the closely related but separate _events_ (aka notifications) category.

## Overview
Define your commands by inheriting from the `ICommand` interface; your queries by inheriting from the `IQuery<TResult>` interface; and your events by inheriting from the `IEvent` interface. Aside from making the roles explicit, these marker interfaces are meant to enable the automatic wire-up of messages with handlers by the dispatcher.

Each type of message is handled by a class that implements that message's respective handler interface. That is, `ICommandHandler<TCommand>` for commands, `IQueryHandler<TQuery, TResult>` for queries, and `IEventHandler<TEvent>` for events. A command and a query will have exactly one handler associated with them, whereas an event can be handled by multiple handlers, or none at all.

Furthermore, command and query handlers can be wrapped by _decorators_ to create processing pipelines for easy extensibility. For this purpose, the `ICommandHandlerWrapper<TCommand>` and `IQueryHandlerWrapper<TQuery, TResult>` interfaces are provided.

Other than calling the `AddCqrs` extension method on `IServiceCollection`, no additional registration is required for the dispatcher to be able to resolve your handlers and wrappers at runtime.

## Usage
A simple command and its associated handler can be defined as follows.

```csharp
public class CreateUserAccount : ICommand
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public RedactableString Password { get; set; }
}

public class CreateUserAccountHandler : ICommandHandler<CreateUserAccount>
{
    public async Task HandleAsync(CreateUserAccount command, CancellationToken cancellationToken)
    {
        // Create a new account given the username, email and password supplied in 
        // the command and store it, possibly using an injected repository.
    }
}
```

Similarly, a query and its handler to retrieve the accounts created by the previous command can be defined in the following manner.

```csharp
public class GetUserAccount : IQuery<UserAccount> // Will only ever match a single user account, or none.
{
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class GetUserAccountHandler : IQueryHandler<GetUserAccount, UserAccount>
{
    public async Task<UserAccount> HandleAsync(GetUserAccount query, CancellationToken cancellationToken)
    {
        // Retrieve the account identified by the username and/or email supplied in the query.
        // Again, possibly by using any services that have been injected through the constructor.
    }
}
```

After registering the dependencies in the `ConfigureServices` method of your `Startup.cs`, you can start using them in your code through regular constructor injection.

```csharp
// Scans the calling assembly for handlers to automatically register.
services.AddCqrs();
```
The `AddCqrs` method accepts the lifetimes that the dispatcher and the handlers will be registered with as optional parameters. Both of these parameters default to `ServiceLifetime.Scoped` when not otherwise specified. It also accepts a `params` array of `Assembly` objects denoting the assemblies in which your handlers and wrappers are defined. If no assemblies are specified, it will default to the assembly from which the method is being called.

The following example shows a sample consumer using the `ICommandDispatcher` and `IQueryDispatcher` dependencies.

```csharp
// Note, you could also just pass in a single IDispatcher instance.
public AccountController(ICommandDispatcher commands, IQueryDispatcher queries)
{
    this.commands = commands;
    this.queries = queries;
}

public async Task<ActionResult> SignUpAsync(string userName, string email, string password)
{
    await commands.DispatchAsync(new CreateUserAccount
    { 
       UserName = userName,
       Email = email,
       Password = password 
    });
    
    // return Success ActionResult..
}

public async Task<ActionResult<UserAccount>> LookupAsync(string userName)
{
    var account = await queries.DispatchAsync(new GetUserAccount 
    {
        UserName = userName
    });
    
    // return the appropriate ActionResult, NotFound, ...
}
```
### Sample decorator
To ensure that the email address of the account to create has not yet been registered, something along the lines of the following decorator can be defined.

```csharp
public class EnsureUniqueEmail : ICommandHandlerWrapper<CreateUserAccount>
{
    private readonly IRepository<UserAccount> repository;
    
    public EnsureUniqueEmail(IRepository<UserAccount> repository)
    {
        this.repository = repository;
    }
    
    public async Task HandleAsync(CreateUserAccount command, HandlerDelegate next, CancellationToken cancellationToken)
    {
        if (await ExistsAsync(command.Email, cancellationToken))
        {
            throw new EmailAlreadyRegisteredException(command.Email);
        }
        
        await next();
    }
    
    private async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
    {
        return (await repository.FindAsync(ua => ua.Email.Equals(email), cancellationToken)).Any();
    }
}
```
Note that the `EnsureUniqueEmail` class above provides a full example, with an injected [`IRepository<TEntity>`](https://github.com/jimatas/Developist.Core.Persistence) dependency. Just like the handlers, any wrappers wil also be automatically resolved by the dispatcher, including any of their dependencies.

## Further examples
Please see the unit tests and samples projects inside the solution for more examples and usage patterns.
