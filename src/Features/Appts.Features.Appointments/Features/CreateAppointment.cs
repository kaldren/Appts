using Appts.Features.Appointments.Infrastructure;
using Appts.Features.Appointments.Models;
using Appts.Shared;
using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace Appts.Features.Appointments.Features;

public class CreateAppointment
{
    [HttpPost("api/appointments")]
    [Authorize]
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

            // Get current user id by using nameidentifier claim
            var clientId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

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
    public record CreateAppointmentRequestModel(string Title, DateTimeOffset Start, DateTimeOffset End, Guid OwnerId);
    public record CreateAppointmentResponseModel(string Title, DateTimeOffset Start, DateTimeOffset End);
    public record CreateAppointmentCommand(CreateAppointmentRequestModel Model) : IRequest<CommandResult<CreateAppointmentResponseModel>>;
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CommandResult<CreateAppointmentResponseModel>>
    {
        private readonly IAppointmentsDb _appointmentsDb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateAppointmentCommandHandler(IAppointmentsDb appointmentsDb, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentsDb = appointmentsDb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<CreateAppointmentResponseModel>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            Guid clientIdGuid;

            // Check if the client is logged in
            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated != true)
                return CommandResult<CreateAppointmentResponseModel>.Failure("You must be logged in to create an appointment");

            string? clientIdClaim = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            clientIdGuid = new Guid(clientIdClaim!);

            // Check if the appointment already exists
            var appointmentExists = await _appointmentsDb.AppointmentExistsAsync(x => x.Title == request.Model.Title && x.Start == request.Model.Start && x.End == request.Model.End, cancellationToken);

            // If the appointment already exists, return command result with error message
            if (appointmentExists)
                return CommandResult<CreateAppointmentResponseModel>.Failure("Appointment already exists");

            // If start date is greater than end date, return command result with error message
            if (request.Model.Start > request.Model.End)
                return CommandResult<CreateAppointmentResponseModel>.Failure("Start date cannot be greater than end date");

            // You cannot create an appointment for yourself
            if (request.Model.OwnerId == clientIdGuid)
                return CommandResult<CreateAppointmentResponseModel>.Failure("You cannot create an appointment for yourself");


            // Add the appointment to the database
            var result = await _appointmentsDb.AddAppointmentAsync(new Appointment
            {
                Title = request.Model.Title,
                Start = request.Model.Start,
                End = request.Model.End,
                OwnerId = request.Model.OwnerId,
                ClientId = clientIdGuid
            }, cancellationToken);


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
            RuleFor(x => x.OwnerId).NotEmpty();
        }
    }
}