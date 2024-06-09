//using Appts.Features.Identity.Models;
//using FastEndpoints;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Identity;

//namespace Appts.Features.Identity.Features;

//[HttpGet("api/identity/roles")]
//public class GetUserRolesEndpoint : Endpoint<Results<Ok<string[]>, ProblemDetails>>
//{
//    private readonly UserManager<ApplicationUser> _userManager;

//    public GetUserRolesEndpoint(UserManager<ApplicationUser> userManager)
//    {
//        _userManager = userManager;
//    }

//    public override async Task<Results<Ok<string[]>, ProblemDetails>> ExecuteAsync(CancellationToken cancellationToken)
//    {
//        var user = await _userManager.GetUserAsync(HttpContext.User);

//        if (user == null)
//        {
//            return new NotFound();
//        }

//        var roles = await _userManager.GetRolesAsync(user);

//        return TypedResults.Ok(roles.ToArray());
//    }
//}