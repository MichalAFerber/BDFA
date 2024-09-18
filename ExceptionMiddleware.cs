using BDFA.BL;
using System.Net;
using System.Text;

namespace BDFA
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request for {Path}", httpContext.Request.Path);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorDetails = new StringBuilder();
            errorDetails.AppendLine("<h1>Error Details</h1>");
            errorDetails.AppendLine($"<p>{exception.Message}</p>");
            if (exception.InnerException != null)
            {
                errorDetails.AppendLine($"<p>Inner Exception: {exception.InnerException.Message}</p>");
            }
            errorDetails.AppendLine($"<p>Request Method: {context.Request.Method}</p>");
            errorDetails.AppendLine($"<p>Request Path: {context.Request.Path}</p>");
            errorDetails.AppendLine($"<p>Request Query String: {context.Request.QueryString}</p>");
            errorDetails.AppendLine($"<p>Source: {exception.Source}</p>");
            errorDetails.AppendLine($"<p>Stack Trace: {exception.StackTrace}</p>");
            errorDetails.AppendLine($"<p>Target Site: {exception.TargetSite}</p>");
            errorDetails.AppendLine($"<p>Timestamp: {DateTime.Now.ToString()}</p>");

            // Log the error and send an email
            Manager.SendMail("support@ari-integration.com", "BDFA Application Error", errorDetails.ToString());

            // Set the response status code
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Redirect to the error page
            context.Response.Redirect("/Error");

            // Return a completed task
            return Task.CompletedTask;
        }
    }
}