using Appts.Features.Appointments.Features;
using Appts.Features.Appointments.Infrastructure;
using Appts.Features.Appointments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace Appts.Features.Appointments.Tests;

public class CreateAppointmentTests
{
    [Fact]
    public async Task CreateAppointment_AppointmentAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var mockSet = new Mock<DbSet<Appointment>>();
        var mockDbService = new Mock<IAppointmentsDb>();

        mockDbService.Setup(m => m.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createAppointmentHandler = new CreateAppointment.CreateAppointmentCommandHandler(mockDbService.Object);

        var createAppointmentRequest = new CreateAppointment.CreateAppointmentRequestModel(
            "xx",
            new DateTimeOffset(2023, 6, 15, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 6, 15, 10, 0, 0, TimeSpan.Zero)
        );

        var command = new CreateAppointment.CreateAppointmentCommand(createAppointmentRequest);

        // Act
        var result = await createAppointmentHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Appointment already exists", result.FailureReason);
    }
}