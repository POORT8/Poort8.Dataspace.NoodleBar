using FastEndpoints;

namespace Poort8.Dataspace.API.Authorization.Enforce;

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
}

public class Response
{
    public bool Allowed { get; set; }
}
