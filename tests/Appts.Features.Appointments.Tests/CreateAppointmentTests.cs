using Appts.Features.Appointments.Infrastructure;
using Appts.Features.Appointments.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using static Appts.Features.Appointments.Features.CreateAppointment;

namespace Appts.Features.Appointments.Tests;

public class CreateAppointmentTests
{
    private string mockUserId = "695a820f-760d-46f3-a10c-929b702ab7e0";

    [Fact]
    public async Task CreateAppointment_ValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new CreateAppointmentCommand(new CreateAppointmentRequestModel("Test Appointment" + Guid.NewGuid().ToString(), DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), Guid.NewGuid()));
        var cancellationToken = CancellationToken.None;

        var appointmentsDbMock = new Mock<IAppointmentsDb>();
        appointmentsDbMock.Setup(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken))
            .ReturnsAsync(false);
        appointmentsDbMock.Setup(x => x.AddAppointmentAsync(It.IsAny<Appointment>(), cancellationToken))
            .ReturnsAsync(true);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, mockUserId)
                })),
            });

        httpContextAccessorMock.Setup(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
        httpContextAccessorMock.Setup(p => p.HttpContext.User.Claims).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, mockUserId)
                })).Claims);

        var handler = new CreateAppointmentCommandHandler(appointmentsDbMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Payload.Should().NotBeNull();
        result.Payload.Title.Should().Be(request.Model.Title);
        result.Payload.Start.Should().Be(request.Model.Start);
        result.Payload.End.Should().Be(request.Model.End);

        appointmentsDbMock.Verify(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken), Times.Once);
        appointmentsDbMock.Verify(x => x.AddAppointmentAsync(It.IsAny<Appointment>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAppointment_AppointmentExists_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new CreateAppointmentCommand(new CreateAppointmentRequestModel("Existing Appointment", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), Guid.NewGuid()));
        var cancellationToken = CancellationToken.None;

        var appointmentsDbMock = new Mock<IAppointmentsDb>();
        appointmentsDbMock.Setup(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken))
            .ReturnsAsync(true);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }))
            });

        httpContextAccessorMock.Setup(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
        httpContextAccessorMock.Setup(p => p.HttpContext.User.Claims).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, mockUserId)
                })).Claims);

        var handler = new CreateAppointmentCommandHandler(appointmentsDbMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().Be("Appointment already exists");

        appointmentsDbMock.Verify(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken), Times.Once);
        appointmentsDbMock.Verify(x => x.AddAppointmentAsync(It.IsAny<Appointment>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CreateAppointment_StartDateGreaterThanEndDate_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new CreateAppointmentCommand(new CreateAppointmentRequestModel("Test Appointment", DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow, Guid.NewGuid()));
        var cancellationToken = CancellationToken.None;

        var appointmentsDbMock = new Mock<IAppointmentsDb>();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                }))
            });

        httpContextAccessorMock.Setup(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
        httpContextAccessorMock.Setup(p => p.HttpContext.User.Claims).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, mockUserId)
                })).Claims);

        var handler = new CreateAppointmentCommandHandler(appointmentsDbMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().Be("Start date cannot be greater than end date");

        appointmentsDbMock.Verify(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken), Times.Once);
        appointmentsDbMock.Verify(x => x.AddAppointmentAsync(It.IsAny<Appointment>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CreateAppointment_OwnerIdEqualsUserId_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = new Guid(mockUserId);
        var request = new CreateAppointmentCommand(new CreateAppointmentRequestModel("Test Appointment", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), userId));
        var cancellationToken = CancellationToken.None;

        var appointmentsDbMock = new Mock<IAppointmentsDb>();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            });

        httpContextAccessorMock.Setup(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
        httpContextAccessorMock.Setup(p => p.HttpContext.User.Claims).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, mockUserId)
                })).Claims);

        var handler = new CreateAppointmentCommandHandler(appointmentsDbMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().Be("You cannot create an appointment for yourself");

        appointmentsDbMock.Verify(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken), Times.Once);
        appointmentsDbMock.Verify(x => x.AddAppointmentAsync(It.IsAny<Appointment>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Handle_NotAuthenticated_ReturnsFailureResult()
    {
        // Arrange
        var request = new CreateAppointmentCommand(new CreateAppointmentRequestModel("Test Appointment", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), Guid.NewGuid()));
        var cancellationToken = CancellationToken.None;

        var appointmentsDbMock = new Mock<IAppointmentsDb>();

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            });

        var handler = new CreateAppointmentCommandHandler(appointmentsDbMock.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().Be("You must be logged in to create an appointment");

        appointmentsDbMock.Verify(x => x.AppointmentExistsAsync(It.IsAny<Expression<Func<Appointment, bool>>>(), cancellationToken), Times.Never);
        appointmentsDbMock.Verify(x => x.AddAppointmentAsync(It.IsAny<Appointment>(), cancellationToken), Times.Never);
    }
}
