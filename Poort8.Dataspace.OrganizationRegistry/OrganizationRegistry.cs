using Microsoft.EntityFrameworkCore;

namespace Poort8.Dataspace.OrganizationRegistry;
public class OrganizationRegistry : IOrganizationRegistry
{
    private readonly IDbContextFactory<OrganizationContext> _contextFactory;

    public OrganizationRegistry(IDbContextFactory<OrganizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    #region Organization
    public async Task<Organization> CreateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.AddAsync(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<Organization?> ReadOrganization(string identifier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organization = await context.Organizations
            .Include(o => o.Adherence)
            .Include(o => o.AdditionalDetails)
            .Include(o => o.AuthorizationRegistries)
            .Include(o => o.Agreements)
            .Include(o => o.Certificates)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .Include(o => o.Services)
            .FirstOrDefaultAsync(o => o.Identifier == identifier);

        if (organization is not null)
            return organization;

        return await context.Organizations
            .Include(o => o.Adherence)
            .Include(o => o.AdditionalDetails)
            .Include(o => o.AuthorizationRegistries)
            .Include(o => o.Agreements)
            .Include(o => o.Certificates)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .Include(o => o.Services)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == identifier));
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(
        string? name = default,
        string? adherenceStatus = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Where(o => name == default || name == o.Name)
            .Where(o => adherenceStatus == default || adherenceStatus == o.Adherence.Status)
            .Where(o => propertyKey == default || o.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(o => o.Adherence)
            .Include(o => o.AdditionalDetails)
            .Include(o => o.AuthorizationRegistries)
            .Include(o => o.Agreements)
            .Include(o => o.Certificates)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .Include(o => o.Services)
            .ToListAsync();
    }

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .Include(o => o.AuthorizationRegistries)
            .Include(o => o.Agreements)
            .Include(o => o.Certificates)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .Include(o => o.Services)
            .SingleAsync(o => o.Identifier == organization.Identifier);

        context.Entry(organizationEntity).CurrentValues.SetValues(organization);
        context.Entry(organizationEntity.Adherence).CurrentValues.SetValues(organization.Adherence);
        context.Entry(organizationEntity.AdditionalDetails).CurrentValues.SetValues(organization.AdditionalDetails);

        await context.SaveChangesAsync();
        return organizationEntity;
    }

    public async Task<bool> DeleteOrganization(string identifier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstOrDefaultAsync(o => o.Identifier == identifier);

        if (organizationEntity == null) return false;

        context.Remove(organizationEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Agreement
    public async Task<Agreement> AddNewAgreementToOrganization(string organizationId, Agreement agreement)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.Agreements.Add(agreement);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return agreement;
    }

    public async Task<Agreement?> ReadAgreement(string agreementId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Include(e => e.Agreements)
            .SelectMany(o => o.Agreements)
            .SingleOrDefaultAsync(a => a.AgreementId == agreementId);
    }

    public async Task<Agreement> UpdateAgreement(Agreement agreement)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var agreementEntity = await context.Organizations
            .Include(e => e.Agreements)
            .SelectMany(o => o.Agreements)
            .SingleAsync(a => a.AgreementId == agreement.AgreementId);

        context.Entry(agreementEntity).CurrentValues.SetValues(agreement);
        await context.SaveChangesAsync();
        return agreementEntity;
    }

    public async Task<bool> DeleteAgreement(string agreementId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var agreementEntity = await context.Organizations
            .Include(e => e.Agreements)
            .SelectMany(o => o.Agreements)
            .SingleOrDefaultAsync(a => a.AgreementId == agreementId);

        if (agreementEntity is null) return false;

        context.Remove(agreementEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region AuthorizationRegistry
    public async Task<AuthorizationRegistry> AddNewAuthorizationRegistryToOrganization(string organizationId, AuthorizationRegistry authorizationRegistry)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.AuthorizationRegistries.Add(authorizationRegistry);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return authorizationRegistry;
    }

    public async Task<AuthorizationRegistry?> ReadAuthorizationRegistry(string authorizationRegistryId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Include(e => e.AuthorizationRegistries)
            .SelectMany(o => o.AuthorizationRegistries)
            .SingleOrDefaultAsync(a => a.AuthorizationRegistryId == authorizationRegistryId);
    }

    public async Task<AuthorizationRegistry> UpdateAuthorizationRegistry(AuthorizationRegistry authorizationRegistry)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var authorizationRegistryEntity = await context.Organizations
            .Include(e => e.AuthorizationRegistries)
            .SelectMany(o => o.AuthorizationRegistries)
            .SingleAsync(a => a.AuthorizationRegistryId == authorizationRegistry.AuthorizationRegistryId);

        context.Entry(authorizationRegistryEntity).CurrentValues.SetValues(authorizationRegistry);
        await context.SaveChangesAsync();
        return authorizationRegistryEntity;
    }

    public async Task<bool> DeleteAuthorizationRegistry(string authorizationRegistryId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var authorizationRegistryEntity = await context.Organizations
            .Include(e => e.AuthorizationRegistries)
            .SelectMany(o => o.AuthorizationRegistries)
            .SingleOrDefaultAsync(a => a.AuthorizationRegistryId == authorizationRegistryId);

        if (authorizationRegistryEntity is null) return false;

        context.Remove(authorizationRegistryEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Certificate
    public async Task<Certificate> AddNewCertificateToOrganization(string organizationId, Certificate certificate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.Certificates.Add(certificate);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return certificate;
    }

    public async Task<Certificate?> ReadCertificate(string certificateId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Include(e => e.Certificates)
            .SelectMany(o => o.Certificates)
            .SingleOrDefaultAsync(c => c.CertificateId == certificateId);
    }

    public async Task<Certificate> UpdateCertificate(Certificate certificate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var certificateEntity = await context.Organizations
            .Include(e => e.Certificates)
            .SelectMany(o => o.Certificates)
            .SingleAsync(c => c.CertificateId == certificate.CertificateId);

        context.Entry(certificateEntity).CurrentValues.SetValues(certificate);
        await context.SaveChangesAsync();
        return certificateEntity;
    }

    public async Task<bool> DeleteCertificate(string certificateId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var certificateEntity = await context.Organizations
            .Include(e => e.Certificates)
            .SelectMany(o => o.Certificates)
            .SingleOrDefaultAsync(c => c.CertificateId == certificateId);

        if (certificateEntity is null) return false;

        context.Remove(certificateEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Role
    public async Task<OrganizationRole> AddNewRoleToOrganization(string organizationId, OrganizationRole role)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.Roles.Add(role);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return role;
    }

    public async Task<OrganizationRole?> ReadRole(string roleId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Include(e => e.Roles)
            .SelectMany(o => o.Roles)
            .SingleOrDefaultAsync(r => r.RoleId == roleId);
    }

    public async Task<OrganizationRole> UpdateRole(OrganizationRole role)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var roleEntity = await context.Organizations
            .Include(e => e.Roles)
            .SelectMany(o => o.Roles)
            .SingleAsync(r => r.RoleId == role.RoleId);

        context.Entry(roleEntity).CurrentValues.SetValues(role);
        await context.SaveChangesAsync();
        return roleEntity;
    }

    public async Task<bool> DeleteRole(string roleId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var roleEntity = await context.Organizations
            .Include(e => e.Roles)
            .SelectMany(o => o.Roles)
            .SingleOrDefaultAsync(r => r.RoleId == roleId);

        if (roleEntity is null) return false;

        context.Remove(roleEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Property
    public async Task<Property> AddNewPropertyToOrganization(string organizationId, Property property)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.Properties.Add(property);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return property;
    }

    public async Task<Property?> ReadProperty(string propertyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Include(e => e.Properties)
            .SelectMany(o => o.Properties)
            .SingleOrDefaultAsync(p => p.PropertyId == propertyId);
    }

    public async Task<Property> UpdateProperty(Property property)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var propertyEntity = await context.Organizations
            .Include(e => e.Properties)
            .SelectMany(o => o.Properties)
            .SingleAsync(p => p.PropertyId == property.PropertyId);

        context.Entry(propertyEntity).CurrentValues.SetValues(property);
        await context.SaveChangesAsync();
        return propertyEntity;
    }

    public async Task<bool> DeleteProperty(string propertyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var propertyEntity = await context.Organizations
            .Include(e => e.Properties)
            .SelectMany(o => o.Properties)
            .SingleOrDefaultAsync(p => p.PropertyId == propertyId);

        if (propertyEntity is null) return false;

        context.Remove(propertyEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Service
    public async Task<Service> AddNewServiceToOrganization(string organizationId, Service service)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.Services.Add(service);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return service;
    }

    public async Task<Service?> ReadService(string serviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Include(e => e.Services)
            .SelectMany(o => o.Services)
            .SingleOrDefaultAsync(s => s.ServiceId == serviceId);
    }

    public async Task<Service> UpdateService(Service service)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var serviceEntity = await context.Organizations
            .Include(e => e.Services)
            .SelectMany(o => o.Services)
            .SingleAsync(s => s.ServiceId == service.ServiceId);

        context.Entry(serviceEntity).CurrentValues.SetValues(service);
        await context.SaveChangesAsync();
        return serviceEntity;
    }

    public async Task<bool> DeleteService(string serviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var serviceEntity = await context.Organizations
            .Include(e => e.Services)
            .SelectMany(o => o.Services)
            .SingleOrDefaultAsync(s => s.ServiceId == serviceId);

        if (serviceEntity is null) return false;

        context.Remove(serviceEntity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region Audit
    public async Task<IReadOnlyList<AuditRecord>> GetAuditRecords()
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.AuditRecords.ToListAsync();
    }
    #endregion
}