using Appts.UserManagement.Application.Commands;
using Appts.UserManagement.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Appts.UserManagement.Application.Handlers;
internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private readonly IUserManagerService _userManagerService;

    public RegisterUserCommandHandler(IUserManagerService userManagerService)
    {
        _userManagerService = userManagerService;
    }

    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userManagerService.CreateUserAsync(request.model, request.model.Password);

        return result;
    }
}
