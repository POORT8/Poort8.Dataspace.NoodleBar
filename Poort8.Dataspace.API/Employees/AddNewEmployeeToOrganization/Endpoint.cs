using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Exceptions;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Employees.AddNewEmployeeToOrganization;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public const string Name = "Authorization Registry Organizations";
    public const string NameId = "organizationId";
    public const string NameChild = "Employees";
    public const string EndpointSummary = "Add a new employee to an organization";
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
        Post("/api/" + Name.Replace(' ', '-').ToLower() + "/{" + NameId + "}/" + NameChild.ToLower());
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
        string organizationId = Route<string>(NameId)!;
        var organization = await _authorizationRegistry.ReadOrganization(organizationId);
        if (organization == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var entity = Map.ToEntity(request);

        try
        {
            var employee = await _authorizationRegistry.AddNewEmployeeToOrganization(organizationId, entity);
            await SendMapped(employee, 201, ct);
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
