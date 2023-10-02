using Microsoft.EntityFrameworkCore;

namespace Poort8.Dataspace.OrganizationRegistry;
public class OrganizationRegistry : IOrganizationRegistry
{
    private readonly IDbContextFactory<OrganizationContext> _contextFactory;

    public OrganizationRegistry(IDbContextFactory<OrganizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;

        //TODO: Run once on startup
        using var context = _contextFactory.CreateDbContext();
        context.Database.Migrate();
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
        return await context.Organizations
            .Include(o => o.Adherence)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .SingleOrDefaultAsync(o => o.Identifier == identifier);
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return context.Organizations
            .Include(o => o.Adherence)
            .Include(o => o.Roles)
            .Include(o => o.Properties)
            .ToList();
    }

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var organizationEntity = context.Organizations.Update(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<bool> DeleteOrganization(string identifier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var organizationEntity = await context.Organizations.FindAsync(identifier);

        if (identifier == null)  return false;

        context.Remove(organizationEntity!);
        await context.SaveChangesAsync();
        return true;
    }
}
