using MediatR;

namespace Appts.Features.Identity.Events;

public record UserLoggedInEvent(string Email) : INotification;
