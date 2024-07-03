using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.ResourceGroups.CreateResourceGroup;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public const string Name = "ResourceGroups";
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
        Post($"/api/{Name.ToLower()}");
        Options(x => x.WithTags(Name));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WriteResourcesPolicy);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var entity = Map.ToEntity(request);

        var resourceGroup = await _authorizationRegistry.CreateResourceGroup(entity);

        await SendMapped(resourceGroup, 201, ct);
    }
}
