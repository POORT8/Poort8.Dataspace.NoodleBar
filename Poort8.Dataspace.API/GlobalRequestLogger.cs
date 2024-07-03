using FastEndpoints;
using System.Text.Json;

namespace Poort8.Dataspace.API;

public class GlobalRequestLogger : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var logger = context.HttpContext.Resolve<ILogger<GlobalRequestLogger>>();

        logger.LogInformation("P8.inf - Received request, Method: {method}, Path: {path}, Request: {requestFullName}, User: {user}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            context.Request?.GetType().FullName,
            context.HttpContext.User.Identity?.Name);

        try
        {
            if (context.Request is not null)
                logger.LogInformation("P8.inf - Received request object: {request}", JsonSerializer.Serialize(context.Request));
        }
        catch (Exception) { }

        return Task.CompletedTask;
    }
}
