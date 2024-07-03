using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.ResourceGroups.CreateResourceGroup;

public class Mapper : Mapper<Request, Response, ResourceGroup>
{
    public override ResourceGroup ToEntity(Request request)
    {
        return new ResourceGroup(
            request.ResourceGroupId,
            request.UseCase ?? "default",
            request.Name,
            request.Description,
            request.Provider,
            request.Url,
            request.Resources?.Select(
                f => new Resource(f.ResourceId, f.Name, f.Description, f.UseCase, f.Properties?.Select(
                    p => new Resource.ResourceProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? [])).ToList() ?? [],
            request.Properties?.Select(
                p => new ResourceGroup.ResourceGroupProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? []);
    }

    public override Response FromEntity(ResourceGroup resourceGroup)
    {
        return new Response(
            resourceGroup.ResourceGroupId,
            resourceGroup.UseCase,
            resourceGroup.Name,
            resourceGroup.Description,
            resourceGroup.Provider,
            resourceGroup.Url,
            resourceGroup.Resources.Select(
                f => new Resources.CreateResource.Response(f.ResourceId, f.Name, f.Description, f.UseCase, f.Properties.Select(
                    p => new Resources.CreateResource.ResourcePropertyDto(p.Key, p.Value, p.IsIdentifier)).ToList())).ToList(),
            resourceGroup.Properties.Select(
                p => new ResourceGroupPropertyDto(p.Key, p.Value, p.IsIdentifier)).ToList());
    }
}
