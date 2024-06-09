using Appts.Features.Identity.Models;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

[HttpPost("api/identity/logout")]
[AllowAnonymous]
public class LogoutUserEndpoint : Endpoint<LogoutUserModel,
                                   Results<Ok<LogoutUserResponseModel>,
                                       UnauthorizedHttpResult,
                                           ProblemDetails>>
{
    private readonly IMediator _mediator;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LogoutUserEndpoint(IMediator mediator, SignInManager<ApplicationUser> signInManager)
    {
        _mediator = mediator;
        _signInManager = signInManager;
    }

    public override async Task<Results<Ok<LogoutUserResponseModel>,
                                           UnauthorizedHttpResult,
                                           ProblemDetails>> ExecuteAsync(LogoutUserModel request, CancellationToken cancellationToken)
    {
        if (request.empty is not null)
        {
            var result = await _mediator.Send(new LogoutUserCommand(request));

            return TypedResults.Ok(new LogoutUserResponseModel());
        }

        return TypedResults.Unauthorized();
    }
}

public record LogoutUserModel(object empty);

public record LogoutUserResponseModel();

public record LogoutUserCommand(LogoutUserModel model) : IRequest<bool>;

internal class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, bool>
{
    private UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public LogoutUserCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IPublisher publisher, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _publisher = publisher;
        _configuration = configuration;
    }


    public async Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();

        return true;
    }
}