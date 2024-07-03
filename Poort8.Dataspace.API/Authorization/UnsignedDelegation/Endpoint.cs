using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;
using Poort8.Ishare.Core.Models;

namespace Poort8.Dataspace.API.Authorization.UnsignedDelegation;

public class Endpoint : Endpoint<DelegationMask, DelegationEvidence>
{
    private readonly ILogger<Endpoint> _logger;
    private readonly IAuthorizationRegistry _authorizationRegistry;

    public Endpoint(ILoggerFactory loggerFactory, IAuthorizationRegistry authorizationRegistry)
    {
        _logger = loggerFactory.CreateLogger<Endpoint>();
        _authorizationRegistry = authorizationRegistry;
    }

    public override void Configure()
    {
        Post("/api/authorization/unsigned-delegation");
        Options(x => x.WithTags("Authorization"));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
    }

    public override async Task HandleAsync(DelegationMask delegationMask, CancellationToken ct)
    {
        var policy = delegationMask.DelegationRequest.PolicySets[0].Policies[0];

        var (allowed, explainPolicies) = await _authorizationRegistry.ExplainedEnforce(
            delegationMask.DelegationRequest.PolicyIssuer,
            delegationMask.DelegationRequest.Target.AccessSubject,
            policy.Target.Environment.ServiceProviders[0],
            policy.Target.Actions[0],
            policy.Target.Resource.Identifiers[0],
            policy.Target.Resource.Type,
            policy.Target.Resource.Attributes[0]);

        _logger.LogInformation("P8.inf - Enforce returned {allowed} with {explainPoliciesCount} explain policies", allowed, explainPolicies.Count);

        var delegationEvidence = DelegationEvidenceCreator.Create(allowed, delegationMask, explainPolicies.Count > 0 ? explainPolicies[0] : null);

        Response = delegationEvidence;
        return;
    }
}
