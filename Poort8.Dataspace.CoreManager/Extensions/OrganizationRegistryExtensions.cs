using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class OrganizationRegistryExtensions
{
    public static Organization DeepCopy(this Organization organization)
    {
        return new Organization(
            organization.Identifier,
            organization.Name,
            new Adherence(organization.Adherence.Status, organization.Adherence.ValidFrom, organization.Adherence.ValidUntil),
            organization.Roles.Select(r => new OrganizationRole(r.Role)).ToList(),
            organization.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList());
    }
}