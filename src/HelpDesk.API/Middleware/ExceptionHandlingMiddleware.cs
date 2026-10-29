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
            _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            ValidationException => (HttpStatusCode.BadRequest, exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            FormatException => (HttpStatusCode.BadRequest, "El formato de los datos ingresados no es válido (ej: GUID inválido o número con formato incorrecto)."),
            JsonException jsonEx => (HttpStatusCode.BadRequest,
                jsonEx.Message.Contains("System.Guid", StringComparison.OrdinalIgnoreCase) || jsonEx.Message.Contains("Guid", StringComparison.OrdinalIgnoreCase)
                    ? "El formato del identificador no es válido. Debe ser un GUID válido (ej: 00000000-0000-0000-0000-000000000000)."
                    : jsonEx.Message.Contains("System.String", StringComparison.OrdinalIgnoreCase)
                        ? "El valor del campo debe ser una cadena de texto (string), no un número ni un objeto."
                        : "El cuerpo de la solicitud contiene formato JSON inválido o tipos de datos incompatibles."),
            BadHttpRequestException badHttpEx => (HttpStatusCode.BadRequest,
                badHttpEx.Message.Contains("non-empty", StringComparison.OrdinalIgnoreCase) || badHttpEx.Message.Contains("body", StringComparison.OrdinalIgnoreCase)
                    ? "El cuerpo de la solicitud no puede estar vacío. Debe enviar un JSON con los datos requeridos."
                    : "La solicitud enviada no tiene un formato válido o contiene datos mal formados."),
            BusinessRuleException => (HttpStatusCode.Conflict, exception.Message),
            DuplicateException => (HttpStatusCode.Conflict, exception.Message),
            DependencyException => (HttpStatusCode.Conflict, exception.Message),
            UnauthorizedActionException => (HttpStatusCode.Forbidden, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, exception.Message),
            ConcurrencyException => (HttpStatusCode.Conflict, exception.Message),
            InvalidOperationException => (HttpStatusCode.Conflict, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = new
            {
                code = statusCode.ToString(),
                message = message
            },
            message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}