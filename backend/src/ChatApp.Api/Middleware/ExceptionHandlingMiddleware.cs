using ChatApp.Application.Exceptions;
using ChatApp.Contracts.Common;
using System.Text.Json;

namespace ChatApp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred while processing request {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var message = GetMessage(exception);

        context.Response.Clear();

        context.Response.StatusCode = statusCode;

        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(
        Exception exception)
    {
        return exception switch
        {
            InvalidCredentialsException =>
                StatusCodes.Status401Unauthorized,

            UserNotAuthenticatedException =>
                StatusCodes.Status401Unauthorized,

            ForbiddenException =>
                StatusCodes.Status403Forbidden,

            NotFoundException =>
                StatusCodes.Status404NotFound,

            ConflictException =>
                StatusCodes.Status409Conflict,

            UserAlreadyExistsException =>
                StatusCodes.Status409Conflict,

            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetMessage(
        Exception exception)
    {
        return exception switch
        {
            InvalidCredentialsException =>
                exception.Message,

            UserNotAuthenticatedException =>
                exception.Message,

            ForbiddenException =>
                exception.Message,

            NotFoundException =>
                exception.Message,

            ConflictException =>
                exception.Message,

            UserAlreadyExistsException =>
                exception.Message,

            _ => "An unexpected error occurred"
        };
    }
}