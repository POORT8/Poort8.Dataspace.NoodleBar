using Microsoft.EntityFrameworkCore;

namespace Poort8.Dataspace.OrganizationRegistry;
public class OrganizationRegistry : IOrganizationRegistry
{
    private readonly IDbContextFactory<OrganizationContext> _contextFactory;

    public OrganizationRegistry(IDbContextFactory<OrganizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

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

        foreach (var authorizationRegistry in organization.AuthorizationRegistries)
        {
            var authorizationRegistryEntity = organizationEntity.AuthorizationRegistries
                .FirstOrDefault(a => a.AuthorizationRegistryId == authorizationRegistry.AuthorizationRegistryId);

            if (authorizationRegistryEntity == null)
            {
                organizationEntity.AuthorizationRegistries.Add(authorizationRegistry);
            }
            else
            {
                context.Entry(authorizationRegistryEntity).CurrentValues.SetValues(authorizationRegistry);
            }
        }

        foreach (var authorizationRegistry in organizationEntity.AuthorizationRegistries)
        {
            if (!organization.AuthorizationRegistries.Any(a => a.AuthorizationRegistryId == authorizationRegistry.AuthorizationRegistryId))
            {
                context.Remove(authorizationRegistry);
            }
        }

        foreach (var agreement in organization.Agreements)
        {
            var agreementEntity = organizationEntity.Agreements
                .FirstOrDefault(a => a.AgreementId == agreement.AgreementId);

            if (agreementEntity == null)
            {
                organizationEntity.Agreements.Add(agreement);
            }
            else
            {
                context.Entry(agreementEntity).CurrentValues.SetValues(agreement);
            }
        }

        foreach (var agreement in organizationEntity.Agreements)
        {
            if (!organization.Agreements.Any(a => a.AgreementId == agreement.AgreementId))
            {
                context.Remove(agreement);
            }
        }

        foreach (var certificate in organization.Certificates)
        {
            var certificateEntity = organizationEntity.Certificates
                .FirstOrDefault(c => c.CertificateId == certificate.CertificateId);

            if (certificateEntity == null)
            {
                organizationEntity.Certificates.Add(certificate);
            }
            else
            {
                context.Entry(certificateEntity).CurrentValues.SetValues(certificate);
            }
        }

        foreach (var certificate in organizationEntity.Certificates)
        {
            if (!organization.Certificates.Any(c => c.CertificateId == certificate.CertificateId))
            {
                context.Remove(certificate);
            }
        }

        foreach (var role in organization.Roles)
        {
            var roleEntity = organizationEntity.Roles
                .FirstOrDefault(r => r.RoleId == role.RoleId);

            if (roleEntity == null)
            {
                organizationEntity.Roles.Add(role);
            }
            else
            {
                context.Entry(roleEntity).CurrentValues.SetValues(role);
            }
        }

        foreach (var role in organizationEntity.Roles)
        {
            if (!organization.Roles.Any(r => r.RoleId == role.RoleId))
            {
                context.Remove(role);
            }
        }

        foreach (var property in organization.Properties)
        {
            var propertyEntity = organizationEntity.Properties
                .FirstOrDefault(p => p.PropertyId == property.PropertyId);

            if (propertyEntity == null)
            {
                organizationEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in organizationEntity.Properties)
        {
            if (!organization.Properties.Any(p => p.PropertyId == property.PropertyId))
            {
                context.Remove(property);
            }
        }

        foreach (var service in organization.Services)
        {
            var serviceEntity = organizationEntity.Services
                .FirstOrDefault(s => s.ServiceId == service.ServiceId);

            if (serviceEntity == null)
            {
                organizationEntity.Services.Add(service);
            }
            else
            {
                context.Entry(serviceEntity).CurrentValues.SetValues(service);
            }
        }

        foreach (var service in organizationEntity.Services)
        {
            if (!organization.Services.Any(s => s.ServiceId == service.ServiceId))
            {
                context.Remove(service);
            }
        }

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

    public async Task<IReadOnlyList<AuditRecord>> GetAuditRecords()
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.AuditRecords.ToListAsync();
    }
}