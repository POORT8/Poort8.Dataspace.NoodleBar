using FastEndpoints;
using FluentValidation;

namespace Poort8.Dataspace.API.Ishare.ConnectToken;

public class Request()
{
    [BindFrom("grant_type")]
    public required string GrantType { get; init; }
    [BindFrom("scope")]
    public required string Scope { get; init; }
    [BindFrom("client_id")]
    public required string ClientId { get; init; }
    [BindFrom("client_assertion_Type")]
    public required string ClientAssertionType { get; init; }
    [BindFrom("client_assertion")]
    public required string ClientAssertion { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.GrantType).Equal("client_credentials");
        RuleFor(x => x.Scope).Equal("iSHARE");
        RuleFor(x => x.ClientAssertionType).Equal("urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
    }
}
