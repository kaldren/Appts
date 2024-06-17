
using Appts.Features.Appointments;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

#region Endpoint
[HttpGet("api/appointments")]
public class GetAllAppointmentsEndpoint : Endpoint<GetAllAppointmentsQuery, Results<Ok<GetAllAppointmentsResponseModel>, NotFound, ProblemHttpResult>>
{
    private readonly IMediator _mediator;

    public GetAllAppointmentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override async Task<Results<Ok<GetAllAppointmentsResponseModel>, NotFound, ProblemHttpResult>> ExecuteAsync(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
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
public record GetAllAppointmentsQuery(Guid Id) : IRequest<GetAllAppointmentsResponseModel>;

public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, GetAllAppointmentsResponseModel>
{
    private readonly AppointmentsDbContext _dbContext;

    public GetAllAppointmentsQueryHandler(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAllAppointmentsResponseModel> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments.FindAsync(request.Id);

        if (appointment == null)
        {
            return null;
        }

        return new GetAllAppointmentsResponseModel
        {
            Id = appointment.Id,
            Title = appointment.Title,
            Start = appointment.Start,
            End = appointment.End
        };
    }
}

#endregion Query