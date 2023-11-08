using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class OrganizationRegistryExtensions
{
    public static Organization DeepCopy(this Organization organization)
    {
        var newOrganization = new Organization(
            organization.Identifier,
            organization.Name)
        {
            Adherence = organization.Adherence.DeepCopy()
        };

        newOrganization.Roles = organization.Roles.Select(r => r.DeepCopy(newOrganization)).ToList();
        newOrganization.Properties = organization.Properties.Select(p => p.DeepCopy(newOrganization)).ToList();

        return newOrganization;
    }

    private static Adherence DeepCopy(this Adherence adherence)
    {
        return new Adherence(
            adherence.Status,
            adherence.ValidFrom,
            adherence.ValidUntil);
    }

    private static OrganizationRole DeepCopy(this OrganizationRole organizationRole, Organization newOrganization)
    {
        return new OrganizationRole(organizationRole.Role)
        {
            RoleId = organizationRole.RoleId,
            OrganizationId = organizationRole.OrganizationId,
            Organization = newOrganization
        };
    }

    private static Property DeepCopy(this Property property, Organization newOrganization)
    {
        return new Property(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            OrganizationId = property.OrganizationId,
            Organization = newOrganization
        };
    }
}