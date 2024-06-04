using Appts.Features.Identity.Events;
using Appts.Features.Identity.Models;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

[HttpPost("api/identity/register")]
[AllowAnonymous]
public class RegisterUserEndpoint : Endpoint<RegisterUserModel,
                                   Results<Ok<RegisterUserResponseModel>,
                                           NotFound,
                                           ProblemDetails>>
{
    private readonly IMediator _mediator;

    public RegisterUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<Results<Ok<RegisterUserResponseModel>,
                                           NotFound,
                                           ProblemDetails>> ExecuteAsync(RegisterUserModel request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request));

        if (result.Succeeded)
        {
            return TypedResults.Ok(new RegisterUserResponseModel(request.Email));
        }

        var errorMessages = result.Errors.Select(e => e.Description).ToList();

        foreach (var error in errorMessages)
        {
            AddError(error);
        }

        return new FastEndpoints.ProblemDetails(ValidationFailures);
    }
}

public record RegisterUserModel(string Email, string Password);

public record RegisterUserResponseModel(string Email);

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
