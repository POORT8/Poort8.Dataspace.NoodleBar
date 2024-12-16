using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Resources.UpdateResource;

public class Endpoint : Endpoint<Request, Response, Mapper>
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
        Put($"/api/{Name.ToLower()}");
        Options(x => x.WithTags(Name));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WriteResourcesPolicy);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var resource = await _authorizationRegistry.ReadResource(request.ResourceId);

        if (resource == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        resource.UseCase = request.UseCase;
        resource.Name = request.Name;
        resource.Description = request.Description;
        resource.Properties = request.Properties?.Select(p => new Resource.ResourceProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? [];

        var updatedResource = await _authorizationRegistry.UpdateResource(resource);

        await SendMapped(updatedResource, 200, ct);
    }
}
