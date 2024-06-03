using Appts.Features.Identity;
using Appts.Features.Identity.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

public record RegisterUserModel(string Email, string Password);

public record RegisterUserCommand(RegisterUserModel model) : IRequest<IdentityResult>;

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private UserManager<ApplicationUser> _userManager;
    private readonly IPublisher _publisher;

    public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, IPublisher publisher)
    {
        _userManager = userManager;
        _publisher = publisher;
    }


    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser { UserName = request.model.Email, Email = request.model.Email };

        var result = await _userManager.CreateAsync(user, request.model.Password);

        //TODO: Update to use Outbox pattern to ensure that the event is published
        if (result.Succeeded)
        {
            await _publisher.Publish(new UserRegisteredEvent(request.model.Email));
        }

        return result;
    }
}
