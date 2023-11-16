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

    #region Create

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.AddAsync(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.AddAsync(product);
        await context.SaveChangesAsync();
        return productEntity.Entity;
    }

    public async Task<Product> CreateProductWithExistingFeatures(Product product, ICollection<string> featureIds)
    {
        var productEntity = await CreateProduct(product);

        foreach (var featureId in featureIds)
        {
            await AddExistingFeatureToProduct(productEntity.ProductId, featureId);
        }

        productEntity = await ReadProduct(productEntity.ProductId);
        return productEntity!;
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

    public async Task<Feature> CreateFeature(Feature feature)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var featureEntity = await context.AddAsync(feature);
        await context.SaveChangesAsync();
        return featureEntity.Entity;
    }

    public async Task<Feature> AddExistingFeatureToProduct(string productId, string featureId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.Products
            .FirstAsync(p => p.ProductId == productId);
        var featureEntity = await context.Features
            .FirstAsync(f => f.FeatureId == featureId);
        productEntity.Features.Add(featureEntity);

        context.Update(productEntity);
        await context.SaveChangesAsync();
        return featureEntity;
    }

    public async Task<Feature> AddNewFeatureToProduct(string productId, Feature feature)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.Products
            .FirstAsync(p => p.ProductId == productId);
        productEntity.Features.Add(feature);

        context.Update(productEntity);
        await context.SaveChangesAsync();
        return feature;
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
            .Where(e => organizationId == default || organizationId == organizationEntityIdentifier)
            .Where(e => familyName == default || familyName == e.FamilyName)
            .Where(e => email == default || email == e.Email)
            .Where(e => propertyKey == default || e.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(e => e.Properties)
            .ToListAsync();
    }

    public async Task<Product?> ReadProduct(string productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var product = await context.Products
            .Include(p => p.Features)
            .ThenInclude(f => f.Properties)
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product is not null)
            return product;

        return await context.Products
            .Include(p => p.Features)
            .ThenInclude(f => f.Properties)
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == productId));
    }

    public async Task<IReadOnlyList<Product>> ReadProducts(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Products
            .Where(p => name == default || name == p.Name)
            .Where(p => propertyKey == default || p.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(p => p.Features)
            .ThenInclude(f => f.Properties)
            .Include(p => p.Properties)
            .ToListAsync();
    }

    public async Task<Feature?> ReadFeature(string featureId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var feature = await context.Features
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(e => e.FeatureId == featureId);

        if (feature is not null)
            return feature;

        return await context.Features
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == featureId));
    }

    public async Task<IReadOnlyList<Feature>> ReadFeatures(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Features
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
                .FirstOrDefault(p => p.Key == property.Key);

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
            if (!organization.Properties.Any(p => p.Key == property.Key))
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
                .FirstOrDefault(p => p.Key == property.Key);

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
            if (!employee.Properties.Any(p => p.Key == property.Key))
            {
                context.Remove(property);
            }
        }
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.Products
            .Include(p => p.Features)
            .ThenInclude(f => f.Properties)
            .Include(p => p.Properties)
            .SingleAsync(p => p.ProductId == product.ProductId);

        context.Entry(productEntity).CurrentValues.SetValues(product);

        foreach (var feature in product.Features)
        {
            var featureEntity = productEntity.Features
                .FirstOrDefault(f => f.FeatureId == feature.FeatureId);

            if (featureEntity == null)
            {
                productEntity.Features.Add(feature);
            }
            else
            {
                context.Entry(featureEntity).CurrentValues.SetValues(feature);
                UpdateFeatureProperties(context, feature, featureEntity);
            }
        }

        foreach (var feature in productEntity.Features)
        {
            if (!product.Features.Any(p => p.FeatureId == feature.FeatureId))
            {
                context.Remove(feature);
            }
        }

        foreach (var property in product.Properties)
        {
            var propertyEntity = productEntity.Properties
                .FirstOrDefault(p => p.Key == property.Key);

            if (propertyEntity == null)
            {
                productEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in productEntity.Properties)
        {
            if (!product.Properties.Any(p => p.Key == property.Key))
            {
                context.Remove(property);
            }
        }

        await context.SaveChangesAsync();
        return productEntity;
    }

    public async Task<Feature> UpdateFeature(Feature feature)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var featureEntity = await context.Features
            .Include(f => f.Properties)
            .SingleAsync(f => f.FeatureId == feature.FeatureId);

        context.Entry(featureEntity).CurrentValues.SetValues(feature);
        UpdateFeatureProperties(context, feature, featureEntity);

        await context.SaveChangesAsync();
        return featureEntity;
    }

    private static void UpdateFeatureProperties(AuthorizationContext context, Feature feature, Feature featureEntity)
    {
        foreach (var property in feature.Properties)
        {
            var propertyEntity = featureEntity.Properties
                .FirstOrDefault(p => p.Key == property.Key);

            if (propertyEntity == null)
            {
                featureEntity.Properties.Add(property);
            }
            else
            {
                context.Entry(propertyEntity).CurrentValues.SetValues(property);
            }
        }

        foreach (var property in featureEntity.Properties)
        {
            if (!feature.Properties.Any(p => p.Key == property.Key))
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

    public async Task<bool> DeleteProduct(string productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.Products
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (productEntity == null) return false;

        context.Remove(productEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteFeature(string featureId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var featureEntity = await context.Features
            .FirstOrDefaultAsync(f => f.FeatureId == featureId);

        if (featureEntity == null) return false;

        context.Remove(featureEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFeatureFromProduct(string productId, string featureId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var product = await context.Products
            .Include(p => p.Features)
            .FirstAsync(p => p.ProductId == productId);
        var features = product.Features as List<Feature>;
        var feature = features!.FirstOrDefault(f => f.FeatureId == featureId);

        if (feature == null) return false;

        features!.Remove(feature);
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

    public async Task<IReadOnlyList<AuditRecord>> ReadAuditRecords()
    {
        using var context = _contextFactory.CreateDbContext();

        return await context.AuditRecords.ToListAsync();
    }
}
