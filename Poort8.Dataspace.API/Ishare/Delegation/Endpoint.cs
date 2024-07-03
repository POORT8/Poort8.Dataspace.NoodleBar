using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.Identity;
using Poort8.Ishare.Core;
using Poort8.Ishare.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Poort8.Dataspace.API.Ishare.Delegation;

public class Endpoint : Endpoint<DelegationMask, DelegationResponse>
{
    private readonly ILogger<Endpoint> _logger;
    private readonly IAuthorizationRegistry _authorizationRegistry;
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public Endpoint(ILoggerFactory loggerFactory, IAuthorizationRegistry authorizationRegistry)
    {
        _logger = loggerFactory.CreateLogger<Endpoint>();
        _authorizationRegistry = authorizationRegistry;
    }

    public override void Configure()
    {
        Post("/api/ishare/delegation");
        Options(x => x.WithTags("iSHARE"));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
    }

    public override async Task HandleAsync(DelegationMask delegationMask, CancellationToken ct)
    {
        var tokenCreator = Resolve<IClientAssertionCreator>();

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

        var claims = new List<Claim>
        {
            new("delegationEvidence", JsonSerializer.Serialize(delegationEvidence, options: jsonSerializerOptions), JsonClaimValueTypes.Json),
            new("policyId", explainPolicies.Count > 0 ? explainPolicies[0].PolicyId : "N/A")
        };
        var delegationToken = tokenCreator.CreateToken(User.Identity!.Name!, claims);

        Response = new DelegationResponse(delegationToken);
    }
}
