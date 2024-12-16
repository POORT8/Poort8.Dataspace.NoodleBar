using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.ResourceGroups.UpdateResourceGroup;

public class Endpoint : Endpoint<Request, Response, Mapper>
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
        Put($"/api/{Name.ToLower().Replace(" ", "")}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WriteResourcesPolicy);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var resourceGroup = await _authorizationRegistry.ReadResourceGroup(request.ResourceGroupId);

        if (resourceGroup == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        resourceGroup.UseCase = request.UseCase;
        resourceGroup.Name = request.Name;
        resourceGroup.Description = request.Description;
        resourceGroup.Provider = request.Provider;
        resourceGroup.Url = request.Url;
        resourceGroup.Properties = request.Properties?.Select(p => new ResourceGroup.ResourceGroupProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? [];

        var updatedResourceGroup = await _authorizationRegistry.UpdateResourceGroup(resourceGroup);

        await SendMapped(updatedResourceGroup, 200, ct);
    }
}
