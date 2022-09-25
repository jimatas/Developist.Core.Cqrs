using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.Events;
using Developist.Core.Cqrs.Queries;

namespace Developist.Core.Cqrs.Tests.Fixture
{
    public record User(string UserName, string? DisplayName);

    public record GetUserQuery(string UserName) : IQuery<User?>
    {
        public bool IsCaseSensitive { get; set; } = true;
    }

    public record AddUserCommand(string UserName, string FirstName, string LastName) : ICommand
    {
        public string? DisplayName { get; set; }
    }

    public record UserLoggedIn(string UserName) : IEvent;

    public class UserRepository
    {
        private readonly ICollection<User> database = new HashSet<User>(new User[]
        {
            new("WelshD", "Dwayne Welsh"),
            new("MarinH", "Hollie Marin"),
            new("HensleyG", "Glenn Hensley")
        });

        public void Add(string userName, string? displayName)
        {
            database.Add(new(userName, displayName));
        }

        public User? FirstOrDefault(Func<User, bool> predicate)
        {
            return database.FirstOrDefault(predicate);
        }
    }
}
