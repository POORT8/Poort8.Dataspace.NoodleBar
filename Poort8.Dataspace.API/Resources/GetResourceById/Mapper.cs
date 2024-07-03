using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.Resources.GetResourceById;

public class Mapper : ResponseMapper<Response, Resource>
{
    public override Response FromEntity(Resource resource)
    {
        return new Response(
            resource.ResourceId,
            resource.UseCase,
            resource.Name,
            resource.Description,
            resource.Properties.Select(
                p => new ResourcePropertyDto(p.Key, p.Value, p.IsIdentifier)).ToList());
    }
}
