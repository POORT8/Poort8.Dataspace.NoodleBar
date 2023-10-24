using Microsoft.EntityFrameworkCore;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationRegistry : IAuthorizationRegistry
{
    private readonly IDbContextFactory<AuthorizationContext> _contextFactory;

    public AuthorizationRegistry(IDbContextFactory<AuthorizationContext> dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    public async Task RunMigrations()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }

    #region Create

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations.AddAsync(organization);
        await context.SaveChangesAsync();
        return organizationEntity.Entity;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return productEntity.Entity;
    }

    public async Task<Policy> CreatePolicy(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.Policies.AddAsync(policy);
        await context.SaveChangesAsync();
        return policyEntity.Entity;
    }

    public async Task<Employee> AddEmployee(string organizationId, Employee employee)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organization = await context.Organizations
            .FirstAsync(o => o.Identifier == organizationId);
        organization.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<Feature> AddFeature(string productId, Feature feature)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var product = await context.Products
            .FirstAsync(p => p.ProductId == productId);
        product.Features.Add(feature);
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
            .Include(o => o.Properties)
            .FirstOrDefaultAsync(o => o.Identifier == identifier);

        if (organization is not null)
            return organization;

        return await context.Organizations
            .Include(o => o.Employees)
            .Include(o => o.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == identifier));
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Organizations
            .Where(o => name == default || name == o.Name)
            .Where(o => propertyKey == default || o.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(o => o.Employees)
            .Include(o => o.Properties)
            .ToListAsync();
    }

    public async Task<Employee?> ReadEmployee(string employeeId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var employee = await context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employee is not null)
            return employee;

        return await context.Employees
            .Include(e => e.Organization)
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
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

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
            .Include(e => e.Organization)
            .Include(e => e.Properties)
            .ToListAsync();
    }

    public async Task<Product?> ReadProduct(string productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var product = await context.Products
            .Include(p => p.Features)
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product is not null)
            return product;

        return await context.Products
            .Include(p => p.Features)
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == productId));
    }

    public async Task<IReadOnlyList<Product>> ReadProducts(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Products
            .Where(p => name == default || name == p.Name)
            .Where(p => propertyKey == default || p.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(p => p.Features)
            .Include(p => p.Properties)
            .ToListAsync();
    }

    public async Task<Feature?> ReadFeature(string featureId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var feature = await context.Features
            .Include(e => e.Products)
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(e => e.FeatureId == featureId);

        if (feature is not null)
            return feature;

        return await context.Features
            .Include(e => e.Products)
            .Include(e => e.Properties)
            .FirstOrDefaultAsync(p => p.Properties.Any(p => p.IsIdentifier && p.Value == featureId));
    }

    public async Task<IReadOnlyList<Feature>> ReadFeatures(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Features
            .Where(f => name == default || name == f.Name)
            .Where(f => propertyKey == default || f.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(f => f.Products)
            .Include(f => f.Properties)
            .ToListAsync();
    }

    public async Task<Policy?> ReadPolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Policies
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
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Policies
            .Where(p => useCase == default || useCase == p.UseCase)
            .Where(p => issuerId == default || issuerId == p.IssuerId)
            .Where(p => subjectId == default || subjectId == p.SubjectId)
            .Where(p => resourceId == default || resourceId == p.ResourceId)
            .Where(p => action == default || action == p.Action)
            .Where(p => propertyKey == default || p.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .ToListAsync();
    }

    #endregion
    #region Update

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .Include(o => o.Properties)
            .Include(o => o.Employees)
            .FirstAsync(o => o.Identifier == organization.Identifier);

        context.Organizations.Remove(organizationEntity);
        var updatedOrganizationEntity = await context.Organizations.AddAsync(organization);
        await context.SaveChangesAsync();
        return updatedOrganizationEntity.Entity;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var employeeEntity = await context.Employees
            .Include(e => e.Organization)
            .FirstAsync(e => e.EmployeeId == employee.EmployeeId);

        var organization = employeeEntity.Organization;

        context.Employees.Remove(employeeEntity);
        organization.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var productEntity = await context.Products
            .FirstAsync(p => p.ProductId == product.ProductId);

        context.Products.Remove(productEntity);
        var updatedProductEntity = await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return updatedProductEntity.Entity;
    }

    public async Task<Feature> UpdateFeature(Feature feature)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var featureEntity = await context.Features
            .Include(f => f.Products)
            .FirstAsync(f => f.FeatureId == feature.FeatureId);

        var products = featureEntity.Products;

        context.Features.Remove(featureEntity);
        foreach (var product in products)
        {
            product.Features.Add(feature);
        }

        await context.SaveChangesAsync();
        return feature;
    }

    public async Task<Policy> UpdatePolicy(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        context.Policies.Remove(policy);
        var updatedPolicyEntity = await context.Policies.AddAsync(policy);
        await context.SaveChangesAsync();
        return updatedPolicyEntity.Entity;
    }

    #endregion
    #region Delete

    public async Task<bool> DeleteOrganization(string identifier)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var organizationEntity = await context.Organizations
            .Include(o => o.Employees)
            .Include(o => o.Properties)
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
            .Include(e => e.Organization)
            .Include(e => e.Properties)
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
            .Include(p => p.Features)
            .Include(p => p.Properties)
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
            .Include(f => f.Products)
            .Include(f => f.Properties)
            .FirstOrDefaultAsync(f => f.FeatureId == featureId);

        if (featureId == null) return false;

        context.Remove(featureEntity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var policyEntity = await context.Policies
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);

        if (policyId == null) return false;

        context.Remove(policyEntity);
        await context.SaveChangesAsync();
        return true;
    }

    #endregion
}