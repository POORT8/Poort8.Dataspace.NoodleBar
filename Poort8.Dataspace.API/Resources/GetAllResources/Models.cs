﻿namespace Poort8.Dataspace.API.Resources.GetAllResources;

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
