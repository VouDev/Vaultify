using System.Net;
using System.Text.Json;
using Vaultify.Domain.Exceptions;

namespace Vaultify.API.Middleware;

/// <summary>
/// Middleware for global exception handling in the API
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class
    /// </summary>
    /// <param name="next">The request delegate</param>
    /// <param name="logger">The logger</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ArgumentException _:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Invalid input provided";
                response.Details = exception.Message;
                break;
                
            case InvalidOperationException _:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "The requested operation cannot be completed";
                response.Details = exception.Message;
                break;
                
            case RepositoryException _:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "A database error occurred";
                response.Details = "The operation could not be completed due to a database error";
                break;
                
            case UnauthorizedAccessException _:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized access";
                response.Details = "You do not have permission to perform this action";
                break;
                
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An unexpected error occurred";
                response.Details = "Please try again later or contact support if the issue persists";
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
    
    /// <summary>
    /// Represents an error response returned to the client
    /// </summary>
    private class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the trace ID for troubleshooting
        /// </summary>
        public string TraceId { get; set; }
        
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or sets additional error details
        /// </summary>
        public string Details { get; set; }
    }
} 