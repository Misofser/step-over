using System.Net;
using GoalApi.Exceptions;

namespace GoalApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occurred. TraceId: {TraceId}, Method: {Method}, Path: {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path
            );

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        HttpStatusCode status;
        string? message;

        switch (ex)
        {
            case NotFoundException:
                status = HttpStatusCode.NotFound;
                message = null;
                break;

            case ConflictException:
                status = HttpStatusCode.Conflict;
                message = ex.Message;
                break;

            case AuthenticationException:
                status = HttpStatusCode.Unauthorized;
                message = ex.Message;
                break;

            case BadRequestException:
                status = HttpStatusCode.BadRequest;
                message = ex.Message;
                break;

            case UnauthorizedAccessException:
                status = HttpStatusCode.Unauthorized;
                message = null;
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                message = "Internal server error";
                break;
        }

        context.Response.StatusCode = (int)status;

        return !string.IsNullOrWhiteSpace(message)
            ? context.Response.WriteAsJsonAsync(new { StatusCode = context.Response.StatusCode, Message = message })
            : Task.CompletedTask;
    }
}
