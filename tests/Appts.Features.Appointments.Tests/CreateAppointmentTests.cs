using Appts.Features.Appointments.Features;
using Appts.Features.Appointments.Infrastructure;
using Appts.Features.Appointments.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var mockSet = new Mock<DbSet<Appointment>>();
        var mockDbService = new Mock<IAppointmentsDb>();

        httpContextAccessorMock.Setup(m => m.HttpContext.User.Identity.IsAuthenticated).Returns(true);

        mockDbService.Setup(m => m.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createAppointmentHandler = new CreateAppointment.CreateAppointmentCommandHandler(mockDbService.Object, httpContextAccessorMock.Object);

        var createAppointmentRequest = new CreateAppointment.CreateAppointmentRequestModel(
            "xxx",
            new DateTimeOffset(2023, 6, 05, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 6, 05, 10, 0, 0, TimeSpan.Zero),
            "695a820f-760d-46f3-a10c-929b702ab7e0"
        );

        var command = new CreateAppointment.CreateAppointmentCommand(createAppointmentRequest);

        // Act
        var result = await createAppointmentHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Appointment already exists", result.FailureReason);
    }

    [Fact]
    public async Task CreateAppointment_StartDateGreaterThanEndDate_ShouldReturnError()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var mockSet = new Mock<DbSet<Appointment>>();
        var mockDbService = new Mock<IAppointmentsDb>();

        httpContextAccessorMock.Setup(m => m.HttpContext.User.Identity.IsAuthenticated).Returns(true);

        mockDbService.Setup(m => m.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var createAppointmentHandler = new CreateAppointment.CreateAppointmentCommandHandler(mockDbService.Object, httpContextAccessorMock.Object);

        var createAppointmentRequest = new CreateAppointment.CreateAppointmentRequestModel(
            "xx",
            new DateTimeOffset(2023, 6, 15, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2023, 6, 10, 10, 0, 0, TimeSpan.Zero), // greater than start date
            "123"
        );

        var command = new CreateAppointment.CreateAppointmentCommand(createAppointmentRequest);

        // Act
        var result = await createAppointmentHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Start date cannot be greater than end date", result.FailureReason);
    }
}