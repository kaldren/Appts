using Appts.SharedKernel;

namespace Appts.UserManagement.Domain.Aggregates.UserAggregate;
public class User : Entity<int>, IAggregateRoot
{
    public User(string username, string email)
    {
        Username = username;
        Email = email;
    }

    public string Username { get; private set; }
    public string Email { get; private set; }
}
