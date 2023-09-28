namespace Poort8.Dataspace.OrganizationRegistry;
public interface IOrganizationRegistry
{
    Task<Organization> CreateOrganization(Organization organization);
    Task<Organization?> ReadOrganization(string identifier);
    Task<List<Organization>> ReadOrganizations();
    Task<Organization> UpdateOrganization(Organization organization);
    Task<bool> DeleteOrganization(string identifier);
}