namespace Poort8.Dataspace.CoreManager.API;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/health").WithTags("Health");

        group.MapGet("", () =>
        {
            return TypedResults.Ok("Hello from the Dataspace Core Manager");
        })
        .WithName("Health")
        .WithOpenApi();
    }
}
