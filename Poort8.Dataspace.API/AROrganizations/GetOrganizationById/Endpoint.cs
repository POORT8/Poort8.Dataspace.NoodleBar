using FastEndpoints;
using Poort8.Dataspace.Identity;
using Poort8.Dataspace.AuthorizationRegistry;

namespace Poort8.Dataspace.API.AROrganizations.GetOrganizationById;

public class Endpoint : EndpointWithoutRequest<Response, Mapper>
{
    public const string Name = "Authorization Registry Organizations";
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
        Get($"/api/{Name.Replace(' ', '-').ToLower()}/{{id}}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadAROrganizationsPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string id = Route<string>("id")!;
        var organization = await _authorizationRegistry.ReadOrganization(id);

        if (organization == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendMapped(organization, 200, ct);
    }
}
