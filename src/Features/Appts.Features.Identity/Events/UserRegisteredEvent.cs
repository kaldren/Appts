using MediatR;

namespace Appts.Features.Identity.Events;

public record UserRegisteredEvent(string Email) : INotification;
