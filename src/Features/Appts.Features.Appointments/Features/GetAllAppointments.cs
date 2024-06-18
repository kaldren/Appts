
using Appts.Features.Appointments;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

#region Endpoint
[HttpGet("api/appointments")]
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
            return null;
        }

        return TypedResults.NotFound();
    }
}
#endregion Endpoint

#region Models
public class GetAllAppointmentsResponseModel
{
    public string AppointmentId { get; set; }
    public string Title { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
#endregion Models

#region Query
public record GetAllAppointmentsQuery(Guid OwnerId) : IRequest<GetAllAppointmentsResponseModel>;

public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, GetAllAppointmentsResponseModel>
{
    private readonly AppointmentsDbContext _dbContext;

    public GetAllAppointmentsQueryHandler(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAllAppointmentsResponseModel> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        //    var appointment = await _dbContext.Appointments
        //        .Where(p => p.OwnerId == request.OwnerId)
        //        .Select(p => new GetAllAppointmentsResponseModel
        //        {
        //            AppointmentId = p.Id,
        //            Title = appointment.Title,
        //            Start = appointment.Start,
        //            End = appointment.End
        //        })
        //        .ToListAsync(cancellationToken);

        //    if (appointment == null)
        //    {
        //        return Task.FromResult<GetAllAppointmentsResponseModel>(null);
        //    }

        //    return new GetAllAppointmentsResponseModel
        //    {
        //        Id = appointment.Id,
        //        Title = appointment.Title,
        //        Start = appointment.Start,
        //        End = appointment.End
        //    };
        //}
    }
}

#endregion Query