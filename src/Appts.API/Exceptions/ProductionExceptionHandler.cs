using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Appts.API.Exceptions;

internal sealed class ProductionExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ProductionExceptionHandler> _logger;

    public ProductionExceptionHandler(ILogger<ProductionExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Exception occurred at {Timestamp}. Type: {ExceptionType}, Message: {ExceptionMessage}, StackTrace: {StackTrace}, TraceID: {TraceID}, User: {UserId}, RequestPath: {RequestPath}",
            DateTime.UtcNow,
            exception.GetType().Name,
            exception.Message,
            exception.StackTrace,
            httpContext.TraceIdentifier,
            httpContext.User.Identity.Name ?? "Anonymous",
            httpContext.Request.Path);


        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "We can't handle your request right now. Please try again later.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}