using Appts.UserManagement.Application.Commands;
using Appts.UserManagement.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Appts.UserManagement.Application.Handlers;
internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private readonly UserManager<UserModel> _userManager;

    public RegisterUserCommandHandler(UserManager<UserModel> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserModel { UserName = request.model.Email, Email = request.model.Email };

        var result = await _userManager.CreateAsync(user, request.model.Password);

        return result;
    }
}
