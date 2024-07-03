namespace Poort8.Dataspace.API.Policies.CreatePolicy;

public record Request(
    string? UseCase,
    long? IssuedAt,
    long? NotBefore,
    long? Expiration,
    string? IssuerId,
    string SubjectId,
    string? ServiceProvider,
    string Action,
    string ResourceId,
    string? Type,
    string? Attribute,
    string? License,
    string? Rules,
    ICollection<PolicyProperty>? Properties);

public record PolicyProperty(
    string Key,
    string Value,
    bool IsIdentifier);
