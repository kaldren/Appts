using Appts.Features.Identity.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using static Appts.Features.Identity.Features.GetUserByIdEndpoint;

namespace Appts.Features.Identity.Features;

[HttpGet("api/identity/{UserId}")]
public class GetUserByIdEndpoint : Endpoint<GetUserByIdRequestModel, Results<Ok<GetUserByIdResponseModel>, NotFound>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<Results<Ok<GetUserByIdResponseModel>, NotFound>> ExecuteAsync(GetUserByIdRequestModel request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new GetUserByIdResponseModel(user.Id, user.UserName, user.Email));
    }

    public record GetUserByIdRequestModel(string UserId);
    public record GetUserByIdResponseModel(string Id, string UserName, string Email);
}