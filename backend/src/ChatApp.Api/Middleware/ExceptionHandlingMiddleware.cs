using ChatApp.Contracts.Common;
using System.Text.Json;

namespace ChatApp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next)
    {
        _next = next;
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
            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/json";

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        var response = new ErrorResponse
        {
            Message = exception.Message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}