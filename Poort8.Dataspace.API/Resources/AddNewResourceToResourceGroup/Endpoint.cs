using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Exceptions;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Resources.AddNewResourceToResourceGroup;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public const string Name = "Resource Groups";
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
        Post("/api/" + Name.ToLower().Replace(" ", "") + "/{" + NameId + "}/" + NameChild.ToLower());
        Options(x => x.WithTags(Name));
        Description(x =>
        {
            x.ClearDefaultProduces(200);
            x.Produces<Response>(201);
            x.Produces(404);
            x.Produces(409);
        });

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

        try
        {
            var resource = await _authorizationRegistry.AddNewResourceToResourceGroup(resourceGroupId, entity);
            await SendMapped(resource, 201, ct);
        }
        catch (RepositoryException e)
        {
            if (e.Message.StartsWith(RepositoryException.IdNotUnique))
                ThrowError(e.Message, 409);
        }
        catch (Exception e)
        {
            _logger.LogCritical("P8.crit - Error in endpoint {endpointName}: {msg}", Name, e.Message);
            throw;
        }
    }
}
