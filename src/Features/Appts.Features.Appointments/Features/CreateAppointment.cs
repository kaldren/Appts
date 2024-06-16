using Appts.Features.Appointments.Models;
using FastEndpoints;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

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

            if (result != null)
            {
                return TypedResults.Ok(result);
            }

            AddError("Unable to create an appointment");

            return new ProblemDetails(ValidationFailures);
        }
    }

    public record CreateAppointmentRequestModel(string Title, DateTimeOffset Start, DateTimeOffset End);
    public record CreateAppointmentResponseModel(string Title, DateTimeOffset Start, DateTimeOffset End);
    public record CreateAppointmentCommand(CreateAppointmentRequestModel Model) : IRequest<CreateAppointmentResponseModel>;

    private class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, CreateAppointmentResponseModel>
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