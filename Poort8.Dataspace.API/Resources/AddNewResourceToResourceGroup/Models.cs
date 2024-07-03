namespace Poort8.Dataspace.API.Resources.AddNewResourceToResourceGroup;

public record Request(
    string ResourceId,
    string UseCase,
    string Name,
    string Description,
    ICollection<ResourcePropertyDto>? Properties);

public record ResourcePropertyDto(
    string Key,
    string Value,
    bool IsIdentifier);

public record Response(
    string ResourceId,
    string UseCase,
    string Name,
    string Description,
    ICollection<ResourcePropertyDto>? Properties);
