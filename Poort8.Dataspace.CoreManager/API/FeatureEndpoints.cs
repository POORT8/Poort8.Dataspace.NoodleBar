using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using static Poort8.Dataspace.AuthorizationRegistry.Entities.Feature;

namespace Poort8.Dataspace.CoreManager.API;

public static class FeatureEndpoints
{
    //TODO: Add tests and exception handling
    public static void MapFeatureEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/features").WithTags("Features");

        group.MapGet("/", async (IAuthorizationRegistry ar) =>
        {
            return TypedResults.Ok(await ar.ReadFeatures());
        })
        .WithName("GetAllFeatures")
        .Produces<IReadOnlyList<Feature>>()
        .WithOpenApi();

        group.MapGet("/{id}", async (string id, IAuthorizationRegistry ar) =>
        {
            return TypedResults.Ok(await ar.ReadFeature(id));
        })
        .WithName("GetFeatureById")
        .Produces<Feature>()
        .WithOpenApi();

        group.MapPut("/{id}", (string id, FeatureInput feature, IAuthorizationRegistry ar) =>
        {
            var properties = feature.Properties.Select(p => new FeatureProperty(p.Key, p.Value, p.IsIdentifier)).ToList();
            var featureUpdate = new Feature(feature.FeatureId, feature.Name, feature.Description, properties);
            var featureEntity = ar.UpdateFeature(featureUpdate);
            return TypedResults.Ok(featureEntity);
        })
        .WithName("UpdateFeature")
        .Produces<Feature>()
        .WithOpenApi();

        group.MapPost("/", async (FeatureInput feature, IAuthorizationRegistry ar) =>
        {
            var properties = feature.Properties.Select(p => new FeatureProperty(p.Key, p.Value, p.IsIdentifier)).ToList();
            var featureCreate = new Feature(feature.FeatureId, feature.Name, feature.Description, properties);
            var featureEntity = await ar.CreateFeature(featureCreate);
            return TypedResults.Created(featureEntity.FeatureId, featureEntity);
        })
        .WithName("CreateFeature")
        .Produces<Feature>()
        .WithOpenApi();

        group.MapDelete("/{id}", async (string id, IAuthorizationRegistry ar) =>
        {
            return TypedResults.Ok(await ar.DeleteFeature(id));
        })
        .WithName("DeleteFeature")
        .Produces<bool>()
        .WithOpenApi();
    }
}

public record FeatureInput(string FeatureId, string Name, string Description, List<PropertyInput> Properties);
public record PropertyInput(string Key, string Value, bool IsIdentifier);
