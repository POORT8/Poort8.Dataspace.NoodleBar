using FastEndpoints;
using Poort8.Dataspace.Identity;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.API.OROrganizations.GetOrganizationById;

public class Endpoint : EndpointWithoutRequest<Response, Mapper>
{
    public const string Name = "Organization Registry";
    private readonly ILogger<Endpoint> _logger;
    private readonly IOrganizationRegistry _organizationRegistry;

    public Endpoint(
        ILoggerFactory loggerFactory,
        IOrganizationRegistry organizationRegistry)
    {
        _logger = loggerFactory.CreateLogger<Endpoint>();
        _organizationRegistry = organizationRegistry;
    }

    public override void Configure()
    {
        Get($"/api/{Name.Replace(' ', '-').ToLower()}/{{id}}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadOROrganizationsPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string id = Route<string>("id")!;
        var organization = await _organizationRegistry.ReadOrganization(id);

        if (organization == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendMapped(organization, 200, ct);
    }
}
