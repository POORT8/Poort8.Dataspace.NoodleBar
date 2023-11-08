using Microsoft.EntityFrameworkCore;

namespace Poort8.Dataspace.OrganizationRegistry;
public class OrganizationRegistry : IOrganizationRegistry
{
    private readonly IDbContextFactory<OrganizationContext> _contextFactory;

    public OrganizationRegistry(IDbContextFactory<OrganizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    public async Task RunMigrations()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        await context.Database.MigrateAsync();
    }

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations.AddAsync(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<Organization?> ReadOrganization(string identifier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organization = await context.Organizations
            .Include(o => o.Adherence)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .FirstOrDefaultAsync(o => o.Identifier == identifier);

        if (organization is not null)
            return organization;

        return await context.Organizations
            .Include(o => o.Adherence)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
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
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .ToListAsync();
    }

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = context.Update(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
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
}
