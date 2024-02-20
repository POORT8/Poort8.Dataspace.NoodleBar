using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class AuthorizationRegistryExtensions
{
    public static Policy DeepCopy(this Policy policy)
    {
        return new Policy(
            policy.UseCase,
            policy.IssuerId,
            policy.SubjectId,
            policy.ResourceId,
            policy.Action,
            policy.Properties.Select(p => p.DeepCopy()).ToList())
        {
            PolicyId = policy.PolicyId,
            IssuedAt = policy.IssuedAt,
            NotBefore = policy.NotBefore,
            Expiration = policy.Expiration
        };
    }

    private static Policy.PolicyProperty DeepCopy(this Policy.PolicyProperty property)
    {
        return new Policy.PolicyProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }

    public static Product DeepCopy(this Product product)
    {
        return new Product(
            product.ProductId,
            product.Name,
            product.Description,
            product.Provider,
            product.Url,
            product.Properties.Select(p => p.DeepCopy()).ToList())
        {
            Features = product.Features.Select(f => f.DeepCopy()).ToList()
        };
    }

    private static Product.ProductProperty DeepCopy(this Product.ProductProperty property)
    {
        return new Product.ProductProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }

    public static Feature DeepCopy(this Feature feature)
    {
        return new Feature(
            feature.FeatureId,
            feature.Name,
            feature.Description,
            feature.Properties.Select(p => p.DeepCopy()).ToList());
    }

    private static Feature.FeatureProperty DeepCopy(this Feature.FeatureProperty property)
    {
        return new Feature.FeatureProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }
}