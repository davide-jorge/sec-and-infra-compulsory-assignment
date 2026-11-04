using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class MyExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException => 403,
            KeyNotFoundException => 404,
            _ => 400
        };

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Title = exception.Message,
                Status = httpContext.Response.StatusCode
            },
            cancellationToken
        );

        return true;
    }
}