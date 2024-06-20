using Appts.Features.Identity.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using static Appts.Features.Identity.Features.GetCurrentUserEndpoint;

namespace Appts.Features.Identity.Features;

[HttpGet("api/identity/current-user")]
public class GetCurrentUserEndpoint : EndpointWithoutRequest<Results<Ok<GetCurrentUserResponseModel>, NotFound>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetCurrentUserEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<Results<Ok<GetCurrentUserResponseModel>, NotFound>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);

        if (user == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new GetCurrentUserResponseModel(user.Id, user.UserName, user.Email));
    }

    public record GetCurrentUserResponseModel(string Id, string UserName, string Email);
}