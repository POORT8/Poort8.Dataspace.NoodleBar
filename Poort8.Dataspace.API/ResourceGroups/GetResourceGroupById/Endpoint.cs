using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.ResourceGroups.GetResourceGroupById;

public class Endpoint : EndpointWithoutRequest<Response, Mapper>
{
    public const string Name = "Resource Groups";
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
        Get($"/api/{Name.ToLower().Replace(" ", "")}/{{id}}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadResourcesPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string id = Route<string>("id")!;
        var resourceGroup = await _authorizationRegistry.ReadResourceGroup(id);

        if (resourceGroup == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendMapped(resourceGroup, 200, ct);
    }
}
