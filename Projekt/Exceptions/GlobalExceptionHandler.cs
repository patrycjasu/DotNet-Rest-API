using Microsoft.AspNetCore.Diagnostics;

namespace Projekt.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var message = exception.Message;
            _logger.LogError($"Error: {message}");

            var code = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                AccessDeniedException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError,
            };

            httpContext.Response.StatusCode = code;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    code,
                    message
                }, cancellationToken);

            return true;
        }
    }
}
