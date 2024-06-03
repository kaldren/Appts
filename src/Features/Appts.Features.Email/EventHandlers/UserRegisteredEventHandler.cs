using Appts.Features.Identity.Events;
using MediatR;

namespace Appts.Features.Email.EventHandlers;

// TODO: Maybe extract all events to a common project so features don't have to reference each other directly
public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"NEW USER REGISTERED!!! Sending email to {notification.Email} to say hello...");

        return Task.CompletedTask;
    }
}
