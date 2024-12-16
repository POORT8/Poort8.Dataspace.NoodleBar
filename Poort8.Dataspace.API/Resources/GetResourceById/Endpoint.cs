using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Resources.GetResourceById;

public class Endpoint : EndpointWithoutRequest<Response, Mapper>
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
        Get($"/api/{Name.ToLower()}/{{id}}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadResourcesPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string id = Route<string>("id")!;
        var resource = await _authorizationRegistry.ReadResource(id);

        if (resource == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendMapped(resource, 200, ct);
    }
}
