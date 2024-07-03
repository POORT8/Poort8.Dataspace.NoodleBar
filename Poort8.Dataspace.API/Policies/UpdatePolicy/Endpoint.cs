using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Policies.UpdatePolicy;

public class Endpoint : Endpoint<Request, Policy>
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
        Put("/api/policies");
        Options(x => x.WithTags("Policies"));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WritePoliciesPolicy);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var policy = await _authorizationRegistry.ReadPolicy(request.PolicyId);

        if (policy == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (policy.IssuerId.Equals(request.IssuerId, StringComparison.OrdinalIgnoreCase) &&
            policy.IssuerId.Equals(User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
        {
            UpdatedPolicy(request, policy);
        }
        else if (User.IsInRole("CanSetPolicyIssuer"))
        {
            UpdatedPolicy(request, policy);
        }
        else
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        Response = await _authorizationRegistry.UpdatePolicy(policy);
    }

    private static void UpdatedPolicy(Request request, Policy policy)
    {
        policy.UseCase = request.UseCase;
        policy.IssuedAt = request.IssuedAt;
        policy.NotBefore = request.NotBefore;
        policy.Expiration = request.Expiration;
        policy.IssuerId = request.IssuerId;
        policy.SubjectId = request.SubjectId;
        policy.ServiceProvider = request.ServiceProvider;
        policy.Action = request.Action;
        policy.ResourceId = request.ResourceId;
        policy.Type = request.Type;
        policy.Attribute = request.Attribute;
        policy.License = request.License;
        policy.Rules = request.Rules;
        policy.Properties = request.Properties?.Select(p => new Policy.PolicyProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? [];
    }
}
