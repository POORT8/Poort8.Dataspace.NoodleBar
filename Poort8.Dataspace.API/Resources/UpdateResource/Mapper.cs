using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.Resources.UpdateResource;

public class Mapper : Mapper<Request, Response, Resource>
{
    public override Resource ToEntity(Request request)
    {
        throw new NotImplementedException();
    }

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
