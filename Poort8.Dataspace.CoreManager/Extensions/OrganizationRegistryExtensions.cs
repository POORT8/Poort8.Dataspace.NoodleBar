using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class OrganizationRegistryExtensions
{
    public static Organization DeepCopy(this Organization organization)
    {
        return new Organization(
            organization.Identifier,
            organization.Name,
            organization.Adherence.DeepCopy(),
            organization.Roles.Select(r => r.DeepCopy()).ToList(),
            organization.Properties.Select(p => p.DeepCopy()).ToList());
    }

    private static Adherence DeepCopy(this Adherence adherence)
    {
        return new Adherence(
            adherence.Status,
            adherence.ValidFrom,
            adherence.ValidUntil);
    }

    private static OrganizationRole DeepCopy(this OrganizationRole organizationRole)
    {
        return new OrganizationRole(organizationRole.Role)
        {
            RoleId = organizationRole.RoleId
        };
    }

    private static Property DeepCopy(this Property property)
    {
        return new Property(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }
}