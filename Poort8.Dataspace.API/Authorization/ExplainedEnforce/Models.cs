using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.Authorization.ExplainedEnforce;

public class Request
{
    [BindFrom("subject")]
    public required string Subject { get; set; }
    [BindFrom("resource")]
    public required string Resource { get; set; }
    [BindFrom("action")]
    public required string Action { get; set; }
    [BindFrom("useCase")]
    public string UseCase { get; set; } = "default";
    [BindFrom("issuer")]
    public string Issuer { get; set; } = "*";
    [BindFrom("serviceProvider")]
    public string ServiceProvider { get; set; } = "*";
    [BindFrom("type")]
    public string Type { get; set; } = "*";
    [BindFrom("attribute")]
    public string Attribute { get; set; } = "*";
    [BindFrom("context")]
    public string Context { get; set; } = string.Empty;
}

public class Response
{
    public bool Allowed { get; set; }
    public List<Policy> ExplainPolicies { get; set; } = [];
}
