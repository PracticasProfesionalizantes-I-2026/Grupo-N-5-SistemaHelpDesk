using HelpDesk.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace HelpDesk.API.Middleware;

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
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            ValidationException => (HttpStatusCode.BadRequest, exception.Message),
            BusinessRuleException => (HttpStatusCode.Conflict, exception.Message),
            DuplicateException => (HttpStatusCode.Conflict, exception.Message),
            DependencyException => (HttpStatusCode.Conflict, exception.Message),
            UnauthorizedActionException => (HttpStatusCode.Forbidden, exception.Message),
            ConcurrencyException => (HttpStatusCode.Conflict, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = new
            {
                code = statusCode.ToString(),
                message = message
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}