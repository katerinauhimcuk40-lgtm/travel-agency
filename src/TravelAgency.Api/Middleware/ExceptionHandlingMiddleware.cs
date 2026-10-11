using Microsoft.AspNetCore.Mvc;

namespace TravelAgency.Api.Middleware;

/// <summary>
/// Перетворює винятки на відповіді ProblemDetails:
/// KeyNotFoundException → 404, InvalidOperationException → 409, ArgumentException → 400, інше → 500.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Business rule violation"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);
        else
            _logger.LogWarning("Request {Path} failed: {Message}", context.Request.Path, ex.Message);

        if (context.Response.HasStarted)
            return;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError ? "An unexpected error occurred." : ex.Message,
            Instance = context.Request.Path
        };

        context.Response.Clear();
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
    }
}
