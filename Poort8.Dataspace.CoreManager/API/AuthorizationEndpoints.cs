using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.API;

public static class AuthorizationEndpoints
{
    //TODO: Add tests and exception handling
    public static void MapAuthorizationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/authorization").WithTags("Authorization Register");

        group.MapGet("/enforce", async (
             string subject,
             string resource,
             string action,
             IAuthorizationRegistry ar,
             string? useCase = "default") =>
        {
            return TypedResults.Ok(await ar.Enforce(subject, resource, action, useCase!));
        })
        .WithName("Enforce")
        .Produces<IReadOnlyList<bool>>()
        .WithOpenApi();

        group.MapGet("/explained-enforce", async (
             string subject,
             string resource,
             string action,
             IAuthorizationRegistry ar,
             string? useCase = "default") =>
        {
            var (allowed, explainPolicies) = await ar.ExplainedEnforce(subject, resource, action, useCase!);
            return TypedResults.Ok(new ExplainedEnforce(allowed, explainPolicies));
        })
        .WithName("ExplainedEnforce")
        .Produces<IReadOnlyList<ExplainedEnforce>>()
        .WithOpenApi();
    }
}

public record ExplainedEnforce(bool allowed, List<Policy> explainPolicies);
