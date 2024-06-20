
using Appts.Features.Appointments;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

#region Endpoint
[HttpGet("api/appointments/{UserId}")]
public class GetAllAppointmentsEndpoint : Endpoint<GetAllAppointmentsQuery, Results<Ok<List<GetAllAppointmentsResponseModel>>, NotFound, ProblemHttpResult>>
{
    private readonly IMediator _mediator;

    public GetAllAppointmentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override async Task<Results<Ok<List<GetAllAppointmentsResponseModel>>, NotFound, ProblemHttpResult>> ExecuteAsync(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request);

        if (result != null)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.NotFound();
    }
}
#endregion Endpoint

#region Models
public class GetAllAppointmentsResponseModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
#endregion Models

#region Query
public record GetAllAppointmentsQuery(string UserId) : IRequest<List<GetAllAppointmentsResponseModel>>;

public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, List<GetAllAppointmentsResponseModel>>
{
    private readonly AppointmentsDbContext _dbContext;

    public GetAllAppointmentsQueryHandler(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetAllAppointmentsResponseModel>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _dbContext.Appointments
            .Where(p => p.OwnerId == Guid.Parse(request.UserId) || p.ClientId == Guid.Parse(request.UserId))
            .Select(p => new GetAllAppointmentsResponseModel
            {
                Id = p.Id,
                Title = p.Title,
                Start = p.Start,
                End = p.End
            })
            .ToListAsync(cancellationToken);

        if (appointments == null)
        {
            return new List<GetAllAppointmentsResponseModel>();
        }

        return appointments;
    }
}
#endregion Query