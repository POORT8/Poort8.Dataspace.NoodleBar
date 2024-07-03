using FastEndpoints;
using System.Text.Json;

namespace Poort8.Dataspace.API;

public class GlobalResponseLogger : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        var logger = context.HttpContext.Resolve<ILogger<GlobalResponseLogger>>();

        logger.LogInformation("P8.inf - Sent response, Method: {method}, Path: {path}, Status: {status}, Response: {responseFullName}, User: {user}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            context.HttpContext.Response.StatusCode,
            context.Response?.GetType().FullName,
            context.HttpContext.User.Identity?.Name);

        try
        {
            if (context.Response is not null)
                logger.LogInformation("P8.inf - Sent response object: {response}", JsonSerializer.Serialize(context.Response));
        }
        catch (Exception) { }

        return Task.CompletedTask;
    }
}
