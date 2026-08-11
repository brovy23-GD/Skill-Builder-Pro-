using SkillBuilderPro.Core.Data; // 🟢 Points to the correct Core home


namespace SkillBuilderPro.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware.
    /// Catches all unhandled exceptions and returns standardized error responses.
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = exception.Message,
                code = "500",
                timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case DrillNotFoundException dnfe:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new { message = dnfe.Message, code = "404", timestamp = DateTime.UtcNow };
                    break;

                case UnauthorizedUserException uue:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response = new { message = uue.Message, code = "401", timestamp = DateTime.UtcNow };
                    break;

                case ArgumentException ae:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new { message = ae.Message, code = "400", timestamp = DateTime.UtcNow };
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Exception thrown when a drill is not found in the database.
    /// </summary>
    public class DrillNotFoundException : Exception
    {
        public DrillNotFoundException(int id)
            : base($"Drill with ID {id} not found.") { }
    }

    /// <summary>
    /// Exception thrown when a user is not authorized to perform an action.
    /// </summary>
    public class UnauthorizedUserException : Exception
    {
        public UnauthorizedUserException(string message = "User is not authorized.")
            : base(message) { }
    }
}