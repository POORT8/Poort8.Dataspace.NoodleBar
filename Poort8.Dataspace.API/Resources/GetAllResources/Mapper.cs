using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.Resources.GetAllResources;

public class Mapper : ResponseMapper<IReadOnlyList<Response>, IReadOnlyList<Resource>>
{
    public override IReadOnlyList<Response> FromEntity(IReadOnlyList<Resource> resources)
    {
        var response = new List<Response>();

        foreach (var resource in resources)
        {
            response.Add(new Response(
                resource.ResourceId,
                resource.UseCase,
                resource.Name,
                resource.Description,
                resource.Properties.Select(
                    p => new ResourcePropertyDto(p.Key, p.Value, p.IsIdentifier)).ToList()));
        }

        return response;
    }
}
