using Appts.UserManagement.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Appts.UserManagement.Application.Interfaces;
public interface IUserManagerService
{
    Task<IdentityResult> CreateUserAsync(RegisterUserModel userModel, string password);
}
