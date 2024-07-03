using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Policies.GetAllPolicies;

public class Endpoint : EndpointWithoutRequest<IReadOnlyList<Policy>>
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
        Get("/api/policies");
        Options(x => x.WithTags("Policies"));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.ReadPoliciesPolicy);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string issuerId;
        string? issuerIdQuery = Query<string>("issuerId", isRequired: false);
        if (string.IsNullOrEmpty(issuerIdQuery))
            issuerId = User.Identity!.Name!;
        else if (User.IsInRole("CanSetPolicyIssuer"))
            issuerId = issuerIdQuery;
        else
        {
            await SendForbiddenAsync(ct);
            return;
        }

        string? useCase = Query<string>("useCase", isRequired: false);

        Response = await _authorizationRegistry.ReadPolicies(
            useCase: useCase,
            issuerId: issuerId);
    }
}
