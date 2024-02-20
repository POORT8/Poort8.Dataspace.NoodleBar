namespace Poort8.Dataspace.OrganizationRegistry;
public interface IOrganizationRegistry //TODO: Add authorizations for managing the registry itself
{
    //Organization
    Task<Organization> CreateOrganization(Organization organization);
    Task<Organization?> ReadOrganization(string identifier);
    Task<IReadOnlyList<Organization>> ReadOrganizations(string? name = default, string? adherenceStatus = default, string? propertyKey = default, string? propertyValue = default);
    Task<Organization> UpdateOrganization(Organization organization);
    Task<bool> DeleteOrganization(string identifier);

    //Agreement
    Task<Agreement> AddNewAgreementToOrganization(string organizationId, Agreement agreement);
    Task<Agreement?> ReadAgreement(string agreementId);
    Task<Agreement> UpdateAgreement(Agreement agreement);
    Task<bool> DeleteAgreement(string agreementId);

    //Authorization Registry
    Task<AuthorizationRegistry> AddNewAuthorizationRegistryToOrganization(string organizationId, AuthorizationRegistry authorizationRegistry);
    Task<AuthorizationRegistry?> ReadAuthorizationRegistry(string authorizationRegistryId);
    Task<AuthorizationRegistry> UpdateAuthorizationRegistry(AuthorizationRegistry authorizationRegistry);
    Task<bool> DeleteAuthorizationRegistry(string authorizationRegistryId);

    //Certificate
    Task<Certificate> AddNewCertificateToOrganization(string organizationId, Certificate certificate);
    Task<Certificate?> ReadCertificate(string certificateId);
    Task<Certificate> UpdateCertificate(Certificate certificate);
    Task<bool> DeleteCertificate(string certificateId);

    //Role
    Task<OrganizationRole> AddNewRoleToOrganization(string organizationId, OrganizationRole role);
    Task<OrganizationRole?> ReadRole(string roleId);
    Task<OrganizationRole> UpdateRole(OrganizationRole role);
    Task<bool> DeleteRole(string roleId);

    //Property
    Task<Property> AddNewPropertyToOrganization(string organizationId, Property property);
    Task<Property?> ReadProperty(string propertyId);
    Task<Property> UpdateProperty(Property property);
    Task<bool> DeleteProperty(string propertyId);

    //Service
    Task<Service> AddNewServiceToOrganization(string organizationId, Service service);
    Task<Service?> ReadService(string serviceId);
    Task<Service> UpdateService(Service service);
    Task<bool> DeleteService(string serviceId);

    //Audit
    Task<IReadOnlyList<AuditRecord>> GetAuditRecords();
}