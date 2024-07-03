using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.ResourceGroups.GetAllResourceGroups;

public class Endpoint : EndpointWithoutRequest<IReadOnlyList<Response>, Mapper>
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
        Get($"/api/{Name.ToLower()}");
        Options(x => x.WithTags(Name));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadResourcesPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? useCase = Query<string>("useCase", isRequired: false);
        var resourceGroups = await _authorizationRegistry.ReadResourceGroups(useCase);

        await SendMapped(resourceGroups, 200, ct);
    }
}
