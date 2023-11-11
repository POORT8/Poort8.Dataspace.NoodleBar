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

    public async Task<Policy> CreatePolicy(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.AddAsync(policy);
        await context.SaveChangesAsync();
        return policyEntity.Entity;
    }

    public async Task<Policy?> ReadPolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Policies
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);
    }

    public async Task<Policy> UpdatePolicy(Policy policy)
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

    public async Task<bool> DeletePolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.Policies
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);

        if (policyEntity == null) return false;

        context.Remove(policyEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Employee> AddNewEmployeeToOrganization(string organizationId, Employee employee)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organizationEntity.Employees.Add(employee);

        context.Update(organizationEntity);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.AddAsync(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(string? name, string? propertyKey, string? propertyValue)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Where(o => name == default || name == o.Name)
            .Where(o => propertyKey == default || o.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(o => o.Employees)
            .ThenInclude(e => e.Properties)
            .Include(o => o.Properties)
            .ToListAsync();
    }

    public async Task<bool> DeleteEmployee(string employeeId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var employeeEntity = await context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employeeEntity == null) return false;

        context.Remove(employeeEntity);
        await context.SaveChangesAsync();
        return true;
    }
}
