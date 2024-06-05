using Appts.Features.Appointments.Models;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Appts.Features.Appointments.Features;
[HttpPost("api/appointments")]
public class CreateAppointmentEndpoint : Endpoint<CreateAppointmentModel,
                                   Results<Ok<CreateAppointmentResponseModel>,
                                           NotFound,
                                           ProblemDetails>>
{
    private readonly IMediator _mediator;

    public CreateAppointmentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<Results<Ok<CreateAppointmentResponseModel>,
                                           NotFound,
                                           ProblemDetails>> ExecuteAsync(CreateAppointmentModel request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateAppointmentCommand(request));

        if (result != null)
        {
            return TypedResults.Ok(result);
        }

        AddError("Unable to create an appointment");

        return new FastEndpoints.ProblemDetails(ValidationFailures);
    }
}
public record CreateAppointmentModel(string Title, DateTimeOffset Start, DateTimeOffset End);

public record CreateAppointmentResponseModel(string Title, DateTimeOffset Start, DateTimeOffset End);

public record CreateAppointmentCommand(CreateAppointmentModel Model) : IRequest<CreateAppointmentResponseModel>;

internal class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResponseModel>
{
    private readonly AppointmentsDbContext _dbContext;

    public CreateAppointmentCommandHandler(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateAppointmentResponseModel> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Add(new Appointment
        {
            Title = request.Model.Title,
            Start = request.Model.Start,
            End = request.Model.End
        });

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (result)
        {
            return new CreateAppointmentResponseModel(request.Model.Title, request.Model.Start, request.Model.End);
        }
        else
        {
            return null;
        }
    }
}

