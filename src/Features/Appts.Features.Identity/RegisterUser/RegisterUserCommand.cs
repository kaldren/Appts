using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Appts.Features.Identity.RegisterUser;
public record RegisterUserCommand(RegisterUserModel model) : IRequest<IdentityResult>;