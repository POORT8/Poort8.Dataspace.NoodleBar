using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Policies.GetPolicyById;

public class Endpoint : EndpointWithoutRequest<Policy?>
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
        Get("/api/policies/{id}");
        Options(x => x.WithTags("Policies"));
        Description(x => x.Produces(404));

        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadPoliciesPolicy);
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
            Response = policy;
            return;
        }
        else
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
    }
}
