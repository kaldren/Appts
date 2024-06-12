
using Appts.Features.Appointments;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

#region Endpoint
[HttpGet("api/appointments/{id}")]
public class GetAppointmentByIdEndpoint : Endpoint<GetAppointmentByIdQuery, Results<Ok<GetAppointmentByIdResponseModel>, NotFound, ProblemHttpResult>>
{
    private readonly IMediator _mediator;

    public GetAppointmentByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override async Task<Results<Ok<GetAppointmentByIdResponseModel>, NotFound, ProblemHttpResult>> ExecuteAsync(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        throw new Exception("This is a test exception");

        try
        {
            throw new Exception("This is a test exception");

            var result = await _mediator.Send(request);

            if (result != null)
            {
                return TypedResults.Ok(result);
            }

            return TypedResults.NotFound();
        }
        catch (Exception)
        {
            return TypedResults.Problem(detail: "Ship order failed to process.", statusCode: 500);
        }
    }
}
#endregion Endpoint

#region Models
public class GetAppointmentByIdResponseModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
#endregion Models

#region Query
public record GetAppointmentByIdQuery(Guid Id) : IRequest<GetAppointmentByIdResponseModel>;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, GetAppointmentByIdResponseModel>
{
    private readonly AppointmentsDbContext _dbContext;

    public GetAppointmentByIdQueryHandler(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAppointmentByIdResponseModel> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments.FindAsync(request.Id);

        if (appointment == null)
        {
            return null;
        }

        return new GetAppointmentByIdResponseModel
        {
            Id = appointment.Id,
            Title = appointment.Title,
            Start = appointment.Start,
            End = appointment.End
        };
    }
}

#endregion Query