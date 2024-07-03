namespace Poort8.Dataspace.API.ResourceGroups.CreateResourceGroup;

public record Request(
    string ResourceGroupId,
    string UseCase,
    string Name,
    string Description,
    string? Provider,
    string? Url,
    ICollection<Resources.CreateResource.Request>? Resources,
    ICollection<ResourceGroupPropertyDto>? Properties);

public record ResourceGroupPropertyDto(
    string Key,
    string Value,
    bool IsIdentifier);

public record Response(
    string ResourceGroupId,
    string UseCase,
    string Name,
    string Description,
    string? Provider,
    string? Url,
    ICollection<Resources.CreateResource.Response>? Resources,
    ICollection<ResourceGroupPropertyDto>? Properties);
