using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Resources.AddNewResourceToResourceGroup;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public const string Name = "ResourceGroups";
    public const string NameId = "resourceGroupId";
    public const string NameChild = "Resources";
    public const string EndpointSummary = "Add a new resource to a resource group";
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
        Post("/api/" + Name.ToLower() + "/{" + NameId + "}/" + NameChild.ToLower());
        Options(x => x.WithTags(Name));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WriteResourcesPolicy);
        Summary(s => { s.Summary = EndpointSummary; });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        string resourceGroupId = Route<string>(NameId)!;
        var resourceGroup = await _authorizationRegistry.ReadResourceGroup(resourceGroupId);
        if (resourceGroup == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var entity = Map.ToEntity(request);

        var resource = await _authorizationRegistry.AddNewResourceToResourceGroup(resourceGroupId, entity);

        await SendMapped(resource, 201, ct);
    }
}
