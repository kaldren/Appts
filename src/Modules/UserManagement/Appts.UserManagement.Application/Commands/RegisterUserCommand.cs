using Appts.UserManagement.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Appts.UserManagement.Application.Commands;
public record RegisterUserCommand(RegisterUserModel model) : IRequest<IdentityResult>;
