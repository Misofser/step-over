using System.Net;
using GoalApi.Exceptions;

namespace GoalApi.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // TODO: Add logging
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
                case UserNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = null;
                    break;

                case UserAlreadyExistsException:
                    status = HttpStatusCode.Conflict;
                    message = ex.Message;
                    break;
                
                case GoalNotFoundException:
                    status = HttpStatusCode.NotFound;
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
}
