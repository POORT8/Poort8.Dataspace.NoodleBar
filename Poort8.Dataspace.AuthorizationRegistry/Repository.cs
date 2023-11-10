using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class Repository : IRepository
{
    private readonly IDbContextFactory<AuthorizationContext> _contextFactory;

    public Repository(IDbContextFactory<AuthorizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    public async Task<Policy> Create(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.AddAsync(policy);
        await context.SaveChangesAsync();
        return policyEntity.Entity;
    }

    public async Task<Policy?> Read(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Policies
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);
    }

    public async Task<Policy> Update(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.Policies
            .Include(p => p.Properties)
            .SingleAsync(p => p.PolicyId == policy.PolicyId);

        context.Entry(policyEntity).CurrentValues.SetValues(policy);

        foreach (var property in policy.Properties)
        {
            var propertyEntity = policyEntity.Properties
                .FirstOrDefault(p => p.Key == property.Key);

            if (propertyEntity == null)
            {
                policyEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in policyEntity.Properties)
        {
            if (!policy.Properties.Any(p => p.Key == property.Key))
            {
                context.Remove(property);
            }
        }

        await context.SaveChangesAsync();
        return policyEntity;
    }

    public async Task<bool> Delete(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.Policies
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);

        if (policyEntity == null) return false;

        context.Remove(policyEntity);
        await context.SaveChangesAsync();
        return true;
    }
}
