using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;

namespace Poort8.Dataspace.API.Authorization.Enforce;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ILogger<Endpoint> _logger;
    private readonly IAuthorizationRegistry _authorizationRegistry;

    public Endpoint(
        ILoggerFactory loggerFactory,
        IAuthorizationRegistry authorizationRegistry)
    {
        _logger = loggerFactory.CreateLogger<Endpoint>();
        _authorizationRegistry = authorizationRegistry;
    }

    public override void Configure()
    {
        Get("/api/authorization/enforce");
        Options(x => x.WithTags("Authorization"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var allowed = await _authorizationRegistry.Enforce(
            request.Subject,
            request.Resource,
            request.Action,
            request.UseCase);
        Response = new Response { Allowed = allowed };
    }
}
