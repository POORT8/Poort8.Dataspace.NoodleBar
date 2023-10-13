﻿namespace Poort8.Dataspace.OrganizationRegistry;
public interface IOrganizationRegistry //TODO: Add authorizations for managing the registry itself
{
    Task<Organization> CreateOrganization(Organization organization);
    Task<Organization?> ReadOrganization(string identifier);
    Task<IReadOnlyList<Organization>> ReadOrganizations(string? name = default, string? adherenceStatus = default, string? propertyKey = default, string? propertyValue = default);
    Task<Organization> UpdateOrganization(Organization organization);
    Task<bool> DeleteOrganization(string identifier);

    //TOOD: Audit
}