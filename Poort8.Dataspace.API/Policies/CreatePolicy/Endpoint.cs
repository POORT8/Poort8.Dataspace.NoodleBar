using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.Identity;

namespace Poort8.Dataspace.API.Policies.CreatePolicy;

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
        Post("/api/policies");
        Options(x => x.WithTags("Policies"));
        AuthSchemes(AuthenticationConstants.IdentityBearer, AuthenticationConstants.Auth0Jwt);
        Policies(AuthenticationConstants.WritePoliciesPolicy);
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        string issuerId;
        if (string.IsNullOrEmpty(request.IssuerId))
            issuerId = User.Identity!.Name!;
        else if (User.IsInRole("CanSetPolicyIssuer"))
            issuerId = request.IssuerId;
        else
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var policy = new Policy(
            request.UseCase ?? "default",
            request.IssuedAt ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
            request.NotBefore ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
            request.Expiration ?? DateTimeOffset.Now.AddYears(1).ToUnixTimeSeconds(),
            issuerId,
            request.SubjectId,
            request.ServiceProvider,
            request.Action,
            request.ResourceId,
            request.Type,
            request.Attribute,
            request.License,
            request.Rules,
            request.Properties?.Select(p => new Policy.PolicyProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? []);

        Response = await _authorizationRegistry.CreatePolicy(policy);
    }
}
