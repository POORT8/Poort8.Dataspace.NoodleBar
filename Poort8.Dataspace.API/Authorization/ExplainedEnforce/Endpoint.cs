using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.Authorization.ExplainedEnforce;

public class Endpoint : Endpoint<Request, Response>
{
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
        Get("/api/authorization/explained-enforce");
        Options(x => x.WithTags("Authorization"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var allowed = false;
        var explainPolicies = new List<Policy>();
        switch (UseCases.GetAuthorizationModel(request.UseCase))
        {
            case "default":
                (allowed, explainPolicies) = await _authorizationRegistry.ExplainedEnforce(
                    request.Subject,
                    request.Resource,
                    request.Action,
                    request.UseCase);
                break;
            case "ishare":
                (allowed, explainPolicies) = await _authorizationRegistry.ExplainedEnforce(
                    request.Issuer,
                    request.Subject,
                    request.ServiceProvider,
                    request.Action,
                    request.Resource,
                    request.Type,
                    request.Attribute,
                    request.UseCase);
                break;
            case "isharerules":
                (allowed, explainPolicies) = await _authorizationRegistry.ExplainedEnforce(
                    request.Issuer,
                    request.Subject,
                    request.ServiceProvider,
                    request.Action,
                    request.Resource,
                    request.Type,
                    request.Attribute,
                    request.Context,
                    request.UseCase);
                break;
            default:
                await SendErrorsAsync(400, ct);
                break;
        }

        Response = new Response { Allowed = allowed, ExplainPolicies = explainPolicies };
    }
}
