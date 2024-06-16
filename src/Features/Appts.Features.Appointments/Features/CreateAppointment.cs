using Appts.Features.Appointments.Models;
using Appts.Shared;
using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Appts.Features.Appointments.Features;

public class CreateAppointment
{
    [HttpPost("api/appointments")]
    public class CreateAppointmentEndpoint : Endpoint<CreateAppointmentRequestModel,
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
                                               ProblemDetails>> ExecuteAsync(CreateAppointmentRequestModel request, CancellationToken cancellationToken)
        {

            // Validate the request
            var validationResult = new CreateAppointmentValidator().Validate(request);

            if (!validationResult.IsValid)
            {
                validationResult.Errors.ToList().ForEach(x => AddError(x.ErrorMessage));

                return new ProblemDetails(ValidationFailures);
            }

            var result = await _mediator.Send(new CreateAppointmentCommand(request));

            if (result.IsSuccess)
            {
                return TypedResults.Ok(result.Payload);
            }
            else
            {
                return new ProblemDetails(new List<ValidationFailure>
                {
                    new("validation error", result.FailureReason)
                });
            }
        }
    }

    public record CreateAppointmentRequestModel(string Title, DateTimeOffset Start, DateTimeOffset End);
    public record CreateAppointmentResponseModel(string Title, DateTimeOffset Start, DateTimeOffset End);
    public record CreateAppointmentCommand(CreateAppointmentRequestModel Model) : IRequest<CommandResult<CreateAppointmentResponseModel>>;

    private class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CommandResult<CreateAppointmentResponseModel>>
    {
        private readonly AppointmentsDbContext _dbContext;

        public CreateAppointmentCommandHandler(AppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommandResult<CreateAppointmentResponseModel>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Check if the appointment already exists
            var appointmentExists = await _dbContext.Appointments.AnyAsync(x => x.Title == request.Model.Title && x.Start == request.Model.Start && x.End == request.Model.End);

            // If the appointment already exists, return command result with error message
            if (appointmentExists)
                return CommandResult<CreateAppointmentResponseModel>.Failure("Appointment already exists");

            // If start date is greater than end date, return command result with error message
            if (request.Model.Start > request.Model.End)
                return CommandResult<CreateAppointmentResponseModel>.Failure("Start date cannot be greater than end date");

            // Add the appointment to the database
            _dbContext.Appointments.Add(new Appointment
            {
                Title = request.Model.Title,
                Start = request.Model.Start,
                End = request.Model.End
            });

            var result = await _dbContext.SaveChangesAsync() > 0;

            // If the appointment was not added to the database, return command result with error message
            if (!result)
                return CommandResult<CreateAppointmentResponseModel>.Failure("Unable to create an appointment");
            else // If the appointment was added to the database, return command result with the appointment details
                return CommandResult<CreateAppointmentResponseModel>.Success(new CreateAppointmentResponseModel(request.Model.Title, request.Model.Start, request.Model.End));
        }
    }
    private class CreateAppointmentValidator : AbstractValidator<CreateAppointmentRequestModel>
    {
        public CreateAppointmentValidator()
        {
            RuleFor(x => x.Title).NotEmpty().Length(2, 100);
            RuleFor(x => x.Start).NotEmpty();
            RuleFor(x => x.End).NotEmpty();
        }
    }
}