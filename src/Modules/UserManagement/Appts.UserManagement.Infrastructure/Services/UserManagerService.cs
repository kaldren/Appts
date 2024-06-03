using Appts.UserManagement.Application.Interfaces;
using Appts.UserManagement.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Appts.UserManagement.Infrastructure.Services;
public class UserManagerService : IUserManagerService
{
    private UserManager<ApplicationUser> _userManager;

    public UserManagerService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateUserAsync(RegisterUserModel userModel, string password)
    {
        var user = new ApplicationUser { UserName = userModel.Email, Email = userModel.Email };

        return await _userManager.CreateAsync(user, password);
    }
}
