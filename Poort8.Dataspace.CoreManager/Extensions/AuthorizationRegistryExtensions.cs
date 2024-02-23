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
}