using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Resources.AddExistingResourceToResourceGroup;

public class Endpoint : EndpointWithoutRequest
{
    public const string Name = "Resource Groups";
    public const string NameId = "resourceGroupId";
    public const string NameChild = "Resources";
    public const string NameChildId = "resourceId";
    public const string EndpointSummary = "Add an existing resource to a resource group";
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
        Put("/api/" + Name.ToLower().Replace(" ", "") + "/{" + NameId + "}/" + NameChild.ToLower() + "/{" + NameChildId + "}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WriteResourcesPolicy);
        Summary(s => { s.Summary = EndpointSummary; });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string resourceGroupId = Route<string>(NameId)!;
        var resourceGroup = await _authorizationRegistry.ReadResourceGroup(resourceGroupId);
        if (resourceGroup == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        string resourceId = Route<string>(NameChildId)!;
        var resource = await _authorizationRegistry.ReadResource(resourceId);
        if (resource == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await _authorizationRegistry.AddExistingResourceToResourceGroup(resourceGroupId, resourceId);
        await SendOkAsync(ct);
    }
}
