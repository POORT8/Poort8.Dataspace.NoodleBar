using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Policies.DeletePolicy;

public class Endpoint : EndpointWithoutRequest
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
        Delete("/api/policies/{id}");
        Options(x => x.WithTags("Policies"));
        Description(x =>
        {
            x.ClearDefaultProduces(200);
            x.Produces(204);
            x.Produces(404);
        });

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.DeletePoliciesPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string id = Route<string>("id")!;
        var policy = await _authorizationRegistry.ReadPolicy(id);

        if (policy == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (policy.IssuerId.Equals(User.Identity!.Name, StringComparison.OrdinalIgnoreCase) ||
            User.IsInRole("CanSetPolicyIssuer"))
        {
            await _authorizationRegistry.DeletePolicy(id);
            return;
        }
        else
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
    }
}
