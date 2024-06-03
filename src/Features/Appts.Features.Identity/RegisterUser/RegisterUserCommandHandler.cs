using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Appts.Features.Identity.RegisterUser;
internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private UserManager<ApplicationUser> _userManager;

    public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser { UserName = request.model.Email, Email = request.model.Email };

        return await _userManager.CreateAsync(user, request.model.Password);
    }
}
