using Appts.Features.Identity.Models;
using Appts.Features.SharedKernel.Features.Identity;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Appts.Features.Identity.Features;

[HttpGet("api/identity/{UserId}")]
public class GetUserByIdEndpoint : Endpoint<GetUserByIdRequestModel, Results<Ok<GetUserByIdResponseModel>, NotFound>>
{
    private readonly IMediator _mediator;

    public GetUserByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<Results<Ok<GetUserByIdResponseModel>, NotFound>> ExecuteAsync(GetUserByIdRequestModel request, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(request.UserId));

        if (user == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new GetUserByIdResponseModel(user.Id, user.UserName, user.Email));
    }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponseModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<GetUserByIdResponseModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
        {
            return null;
        }

        return new GetUserByIdResponseModel(user.Id, user.UserName, user.Email);
    }
}