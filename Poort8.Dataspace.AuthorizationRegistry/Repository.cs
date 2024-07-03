using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class Repository : IRepository
{
    private readonly IDbContextFactory<AuthorizationContext> _contextFactory;

    public Repository(IDbContextFactory<AuthorizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    #region Create

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.AddAsync(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<ResourceGroup> CreateResourceGroup(ResourceGroup resourceGroup)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroupEntity = await context.AddAsync(resourceGroup);
        await context.SaveChangesAsync();
        return resourceGroupEntity.Entity;
    }

    public async Task<ResourceGroup> CreateResourceGroupWithExistingResources(ResourceGroup resourceGroup, ICollection<string> resourceIds)
    {
        var resourceGroupEntity = await CreateResourceGroup(resourceGroup);

        foreach (var resourceId in resourceIds)
        {
            await AddExistingResourceToResourceGroup(resourceGroupEntity.ResourceGroupId, resourceId);
        }

        resourceGroupEntity = await ReadResourceGroup(resourceGroupEntity.ResourceGroupId);
        return resourceGroupEntity!;
    }

    public async Task<Policy> CreatePolicy(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.AddAsync(policy);
        await context.SaveChangesAsync();
        return policyEntity.Entity;
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

    public async Task<Resource> CreateResource(Resource resource)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceEntity = await context.AddAsync(resource);
        await context.SaveChangesAsync();
        return resourceEntity.Entity;
    }

    public async Task<Resource> AddExistingResourceToResourceGroup(string resourceGroupId, string resourceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroupEntity = await context.ResourceGroups
            .FirstAsync(rg => rg.ResourceGroupId == resourceGroupId);
        var resourceEntity = await context.Resources
            .FirstAsync(r => r.ResourceId == resourceId);
        resourceGroupEntity.Resources.Add(resourceEntity);

        context.Update(resourceGroupEntity);
        await context.SaveChangesAsync();
        return resourceEntity;
    }

    public async Task<Resource> AddNewResourceToResourceGroup(string resourceGroupId, Resource resource)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroupEntity = await context.ResourceGroups
            .FirstAsync(rg => rg.ResourceGroupId == resourceGroupId);
        resourceGroupEntity.Resources.Add(resource);

        context.Update(resourceGroupEntity);
        await context.SaveChangesAsync();
        return resource;
    }

    #endregion
    #region Read

    public async Task<Organization?> ReadOrganization(string identifier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organization = await context.Organizations
            .Include(o => o.Employees)
            .ThenInclude(e => e.Properties)
            .Include(o => o.Properties)
            .FirstOrDefaultAsync(o => o.Identifier == identifier);

        if (organization is not null)
            return organization;

        return await context.Organizations
            .Include(o => o.Employees)
            .ThenInclude(e => e.Properties)
            .Include(o => o.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == identifier));
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(string? useCase, string? name, string? propertyKey, string? propertyValue)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Where(p => useCase == default || useCase == p.UseCase)
            .Where(o => name == default || name == o.Name)
            .Where(o => propertyKey == default || o.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(o => o.Employees)
            .ThenInclude(e => e.Properties)
            .Include(o => o.Properties)
            .ToListAsync();
    }

    public async Task<Employee?> ReadEmployee(string employeeId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var employee = await context.Employees
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employee is not null)
            return employee;

        return await context.Employees
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == employeeId));
    }

    public async Task<IReadOnlyList<Employee>> ReadEmployees(
        string? useCase = default,
        string? organizationId = default,
        string? familyName = default,
        string? email = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntityIdentifier = "";
        if (organizationId != default)
        {
            var organizationEntity = await ReadOrganization(organizationId);
            organizationEntityIdentifier = organizationEntity?.Identifier;
        }

        return await context.Employees
            .Where(p => useCase == default || useCase == p.UseCase)
            .Where(e => organizationId == default || organizationId == organizationEntityIdentifier)
            .Where(e => familyName == default || familyName == e.FamilyName)
            .Where(e => email == default || email == e.Email)
            .Where(e => propertyKey == default || e.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(e => e.Properties)
            .ToListAsync();
    }

    public async Task<ResourceGroup?> ReadResourceGroup(string resourceGroupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroup = await context.ResourceGroups
            .Include(rg => rg.Resources)
            .ThenInclude(r => r.Properties)
            .Include(rg => rg.Properties)
            .FirstOrDefaultAsync(rg => rg.ResourceGroupId == resourceGroupId);

        if (resourceGroup is not null)
            return resourceGroup;

        return await context.ResourceGroups
            .Include(rg => rg.Resources)
            .ThenInclude(r => r.Properties)
            .Include(rg => rg.Properties)
            .FirstOrDefaultAsync(rg => rg.Properties.Any(p => p.IsIdentifier && p.Value == resourceGroupId));
    }

    public async Task<IReadOnlyList<ResourceGroup>> ReadResourceGroups(
        string? useCase = default,
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.ResourceGroups
            .Where(rg => useCase == default || useCase == rg.UseCase)
            .Where(rg => name == default || name == rg.Name)
            .Where(rg => propertyKey == default || rg.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(rg => rg.Resources)
            .ThenInclude(r => r.Properties)
            .Include(rg => rg.Properties)
            .ToListAsync();
    }

    public async Task<Resource?> ReadResource(string resourceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resource = await context.Resources
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(e => e.ResourceId == resourceId);

        if (resource is not null)
            return resource;

        return await context.Resources
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == resourceId));
    }

    public async Task<IReadOnlyList<Resource>> ReadResources(
        string? useCase = default,
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Resources
            .Where(p => useCase == default || useCase == p.UseCase)
            .Where(f => name == default || name == f.Name)
            .Where(f => propertyKey == default || f.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(f => f.Properties)
            .ToListAsync();
    }

    public async Task<Policy?> ReadPolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Policies
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);
    }

    public async Task<IReadOnlyList<Policy>> ReadPolicies(
        string? useCase = null,
        string? issuerId = null,
        string? subjectId = null,
        string? resourceId = null,
        string? action = null,
        string? propertyKey = null,
        string? propertyValue = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Policies
            .Where(p => useCase == default || useCase == p.UseCase)
            .Where(p => issuerId == default || issuerId == p.IssuerId)
            .Where(p => subjectId == default || subjectId == p.SubjectId)
            .Where(p => resourceId == default || resourceId == p.ResourceId)
            .Where(p => action == default || action == p.Action)
            .Where(p => propertyKey == default || p.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(p => p.Properties)
            .ToListAsync();
    }

    #endregion
    #region Update

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .Include(o => o.Employees)
            .ThenInclude(e => e.Properties)
            .Include(o => o.Properties)
            .SingleAsync(o => o.Identifier == organization.Identifier);

        context.Entry(organizationEntity).CurrentValues.SetValues(organization);

        //NOTE: context.Update(organization) does not work as expected
        foreach (var employee in organization.Employees)
        {
            var employeeEntity = organizationEntity.Employees
                .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

            if (employeeEntity == null)
            {
                organizationEntity.Employees.Add(employee);
            }
            else
            {
                context.Entry(employeeEntity).CurrentValues.SetValues(employee);
                UpdateEmployeeProperties(context, employee, employeeEntity);
            }
        }

        foreach (var employee in organizationEntity.Employees)
        {
            if (!organization.Employees.Any(p => p.EmployeeId == employee.EmployeeId))
            {
                context.Remove(employee);
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

        await context.SaveChangesAsync();
        return organizationEntity;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var employeeEntity = await context.Employees
            .Include(e => e.Properties)
            .SingleAsync(e => e.EmployeeId == employee.EmployeeId);

        context.Entry(employeeEntity).CurrentValues.SetValues(employee);
        UpdateEmployeeProperties(context, employee, employeeEntity);

        await context.SaveChangesAsync();
        return employeeEntity;
    }

    private static void UpdateEmployeeProperties(AuthorizationContext context, Employee employee, Employee employeeEntity)
    {
        foreach (var property in employee.Properties)
        {
            var propertyEntity = employeeEntity.Properties
                .FirstOrDefault(p => p.PropertyId == property.PropertyId);

            if (propertyEntity == null)
            {
                employeeEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in employeeEntity.Properties)
        {
            if (!employee.Properties.Any(p => p.PropertyId == property.PropertyId))
            {
                context.Remove(property);
            }
        }
    }

    public async Task<ResourceGroup> UpdateResourceGroup(ResourceGroup resourceGroup)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroupEntity = await context.ResourceGroups
            .Include(rg => rg.Resources)
            .ThenInclude(r => r.Properties)
            .Include(rg => rg.Properties)
            .SingleAsync(rg => rg.ResourceGroupId == resourceGroup.ResourceGroupId);

        context.Entry(resourceGroupEntity).CurrentValues.SetValues(resourceGroup);

        foreach (var resource in resourceGroup.Resources)
        {
            var resourceEntity = resourceGroupEntity.Resources
                .FirstOrDefault(r => r.ResourceId == resource.ResourceId);

            if (resourceEntity == null)
            {
                resourceGroupEntity.Resources.Add(resource);
            }
            else
            {
                context.Entry(resourceEntity).CurrentValues.SetValues(resource);
                UpdateResourceProperties(context, resource, resourceEntity);
            }
        }

        foreach (var resource in resourceGroupEntity.Resources)
        {
            if (!resourceGroup.Resources.Any(r => r.ResourceId == resource.ResourceId))
            {
                context.Remove(resource);
            }
        }

        foreach (var property in resourceGroup.Properties)
        {
            var propertyEntity = resourceGroupEntity.Properties
                .FirstOrDefault(p => p.PropertyId == property.PropertyId);

            if (propertyEntity == null)
            {
                resourceGroupEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in resourceGroupEntity.Properties)
        {
            if (!resourceGroup.Properties.Any(p => p.PropertyId == property.PropertyId))
            {
                context.Remove(property);
            }
        }

        await context.SaveChangesAsync();
        return resourceGroupEntity;
    }

    public async Task<Resource> UpdateResource(Resource resource)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceEntity = await context.Resources
            .Include(f => f.Properties)
            .SingleAsync(f => f.ResourceId == resource.ResourceId);

        context.Entry(resourceEntity).CurrentValues.SetValues(resource);
        UpdateResourceProperties(context, resource, resourceEntity);

        await context.SaveChangesAsync();
        return resourceEntity;
    }

    private static void UpdateResourceProperties(AuthorizationContext context, Resource resource, Resource resourceEntity)
    {
        foreach (var property in resource.Properties)
        {
            var propertyEntity = resourceEntity.Properties
                .FirstOrDefault(p => p.PropertyId == property.PropertyId);

            if (propertyEntity == null)
            {
                resourceEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in resourceEntity.Properties)
        {
            if (!resource.Properties.Any(p => p.PropertyId == property.PropertyId))
            {
                context.Remove(property);
            }
        }
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
                .FirstOrDefault(p => p.PropertyId == property.PropertyId);

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
            if (!policy.Properties.Any(p => p.PropertyId == property.PropertyId))
            {
                context.Remove(property);
            }
        }

        await context.SaveChangesAsync();
        return policyEntity;
    }

    #endregion
    #region Delete

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

    public async Task<bool> DeleteResourceGroup(string resourceGroupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroupEntity = await context.ResourceGroups
            .FirstOrDefaultAsync(p => p.ResourceGroupId == resourceGroupId);

        if (resourceGroupEntity == null) return false;

        context.Remove(resourceGroupEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteResource(string resourceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceEntity = await context.Resources
            .FirstOrDefaultAsync(f => f.ResourceId == resourceId);

        if (resourceEntity == null) return false;

        context.Remove(resourceEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveResourceFromResourceGroup(string resourceGroupId, string resourceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var resourceGroup = await context.ResourceGroups
            .Include(p => p.Resources)
            .FirstAsync(p => p.ResourceGroupId == resourceGroupId);
        var resources = resourceGroup.Resources as List<Resource>;
        var resource = resources!.FirstOrDefault(f => f.ResourceId == resourceId);

        if (resource == null) return false;

        resources!.Remove(resource);
        await context.SaveChangesAsync();
        return true;
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

    #endregion

    public async Task<IReadOnlyList<EntityAuditRecord>> ReadEntityAuditRecords(int numberOfRecords = 100)
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.EntityAuditRecords
            .AsNoTracking()
            .OrderByDescending(r => r.Timestamp)
            .Take(numberOfRecords)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<EnforceAuditRecord>> ReadEnforceAuditRecords(int numberOfRecords = 100)
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.EnforceAuditRecords
            .AsNoTracking()
            .OrderByDescending(r => r.Timestamp)
            .Take(numberOfRecords)
            .ToListAsync();
    }

    public async Task<EnforceAuditRecord> CreateEnforceAuditRecord(string user, string useCase, string subjectId, string resourceId, string action, bool allow, List<Policy>? explains = null)
    {
        using var context = _contextFactory.CreateDbContext();

        var record = new EnforceAuditRecord(user, useCase, subjectId, resourceId, action, allow, explains);

        var enforceAuditRecord = await context.AddAsync(record);
        await context.SaveChangesAsync();
        return enforceAuditRecord.Entity;
    }
}
