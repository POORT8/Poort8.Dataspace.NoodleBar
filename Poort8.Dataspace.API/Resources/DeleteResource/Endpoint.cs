using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Resources.DeleteResource;

public class Endpoint : EndpointWithoutRequest
{
    public const string Name = "Resources";
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
        Delete($"/api/{Name.ToLower()}/{{id}}");
        Options(x => x.WithTags(Name));
        Description(x =>
        {
            x.ClearDefaultProduces(200);
            x.Produces(204);
            x.Produces(404);
        });

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.DeleteResourcesPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string id = Route<string>("id")!;
        var deleted = await _authorizationRegistry.DeleteResource(id);
        if (!deleted)
        {
            await SendNotFoundAsync(ct);
            return;
        }
    }
}
