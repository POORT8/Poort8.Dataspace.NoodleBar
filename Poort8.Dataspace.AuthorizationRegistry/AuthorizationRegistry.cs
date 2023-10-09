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

    //Organization
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
            .Include(o => o.Employees)
            .Include(o => o.Properties)
            .SingleOrDefaultAsync(o => o.Identifier == identifier);
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        var organizations = context.Organizations
            .Where(o => name == default || name == o.Name)
            .Where(o => propertyKey == default || o.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(o => o.Employees)
            .Include(o => o.Properties);

        return organizations.ToList();
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
        var organizationEntity = await context.Organizations
            .Include(o => o.Employees)
            .Include(o => o.Properties)
            .SingleOrDefaultAsync(o => o.Identifier == identifier);

        if (identifier == null) return false;

        context.Remove(organizationEntity!);
        await context.SaveChangesAsync();
        return true;
    }

    //Employee
    public async Task<Employee> CreateEmployee(Employee employee)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var employeeEntity = await context.Employees.AddAsync(employee);
        await context.SaveChangesAsync();
        return employeeEntity.Entity;
    }

    public async Task<Employee?> ReadEmployee(string employeeId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Properties)
            .SingleOrDefaultAsync(e => e.EmployeeId == employeeId);
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

        var employees = context.Employees
            .Where(e => organizationId == default || organizationId == e.OrganizationId)
            .Where(e => familyName == default || familyName == e.FamilyName)
            .Where(e => email == default || email == e.Email)
            .Where(e => propertyKey == default || e.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(e => e.Organization)
            .Include(e => e.Properties);

        return employees.ToList();
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var employeeEntity = context.Employees.Update(employee);
        await context.SaveChangesAsync();
        return employeeEntity.Entity;
    }

    public async Task<bool> DeleteEmployee(string employeeId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var employeeEntity = await context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Properties)
            .SingleOrDefaultAsync(e => e.EmployeeId == employeeId);

        if (employeeId == null) return false;

        context.Remove(employeeEntity!);
        await context.SaveChangesAsync();
        return true;
    }

    //Action
    public async Task<Entities.Action> CreateAction(Entities.Action action)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var actionEntity = await context.Actions.AddAsync(action);
        await context.SaveChangesAsync();
        return actionEntity.Entity;
    }

    public async Task<Entities.Action?> ReadAction(string actionId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Actions.SingleOrDefaultAsync(a => a.ActionId == actionId);
    }

    public async Task<IReadOnlyList<Entities.Action>> ReadActions(
        string? name = default,
        string? additionalType = default)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var actions = context.Actions
            .Where(a => name == default || name == a.Name)
            .Where(a => additionalType == default || additionalType == a.AdditionalType);

        return actions.ToList();
    }

    public async Task<Entities.Action> UpdateAction(Entities.Action action)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var actionEntity = context.Actions.Update(action);
        await context.SaveChangesAsync();
        return actionEntity.Entity;
    }

    public async Task<bool> DeleteAction(string actionId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var actionEntity = await context.Actions
            .SingleOrDefaultAsync(a => a.ActionId == actionId);

        if (actionId == null) return false;

        context.Remove(actionEntity!);
        await context.SaveChangesAsync();
        return true;
    }

    //Product
    public async Task<Product> CreateProduct(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var productEntity = await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return productEntity.Entity;
    }

    public async Task<Product?> ReadProduct(string productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Products
            .Include(p => p.PotentialActions)
            .Include(p => p.Services)
            .Include(p => p.Properties)
            .SingleOrDefaultAsync(p => p.ProductId == productId);
    }

    public async Task<IReadOnlyList<Product>> ReadProducts(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        var products = context.Products
            .Where(p => name == default || name == p.Name)
            .Where(p => propertyKey == default || p.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(p => p.PotentialActions)
            .Include(p => p.Services)
            .Include(p => p.Properties);

        return products.ToList();
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var productEntity = context.Products.Update(product);
        await context.SaveChangesAsync();
        return productEntity.Entity;
    }

    public async Task<bool> DeleteProduct(string productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var productEntity = await context.Products
            .Include(p => p.PotentialActions)
            .Include(p => p.Services)
            .Include(p => p.Properties)
            .SingleOrDefaultAsync(p => p.ProductId == productId);

        if (productId == null) return false;

        context.Remove(productEntity!);
        await context.SaveChangesAsync();
        return true;
    }

    //Service
    public async Task<Service> CreateService(Service service)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var serviceEntity = await context.Services.AddAsync(service);
        await context.SaveChangesAsync();
        return serviceEntity.Entity;
    }

    public async Task<Service?> ReadService(string serviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Services
            .Include(s => s.Products)
            .Include(s => s.Properties)
            .SingleOrDefaultAsync(s => s.ServiceId == serviceId);
    }

    public async Task<IReadOnlyList<Service>> ReadServices(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        var services = context.Services
            .Where(s => name == default || name == s.Name)
            .Where(s => propertyKey == default || s.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value))
            .Include(s => s.Products)
            .Include(s => s.Properties);

        return services.ToList();
    }

    public async Task<Service> UpdateService(Service service)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var serviceEntity = context.Services.Update(service);
        await context.SaveChangesAsync();
        return serviceEntity.Entity;
    }

    public async Task<bool> DeleteService(string serviceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var serviceEntity = await context.Services
            .Include(s => s.Products)
            .Include(s => s.Properties)
            .SingleOrDefaultAsync(s => s.ServiceId == serviceId);

        if (serviceId == null) return false;

        context.Remove(serviceEntity!);
        await context.SaveChangesAsync();
        return true;
    }

    //Policy
    public async Task<Policy> CreatePolicy(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var policyEntity = await context.Policies.AddAsync(policy);
        await context.SaveChangesAsync();
        return policyEntity.Entity;
    }

    public async Task<Policy?> ReadPolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Policies
            .SingleOrDefaultAsync(p => p.PolicyId == policyId);
    }

    public async Task<IReadOnlyList<Policy>> ReadPolicies(
        string? issuer = null,
        string? subject = null,
        string? resource = null,
        string? action = null,
        string? propertyKey = null,
        string? propertyValue = null)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");

        using var context = await _contextFactory.CreateDbContextAsync();

        var policies = context.Policies
            .Where(p => issuer == default || issuer == p.Issuer)
            .Where(p => subject == default || subject == p.Subject)
            .Where(p => resource == default || resource == p.Resource)
            .Where(p => action == default || action == p.Action)
            .Where(p => propertyKey == default || p.Properties.Any(p => propertyKey == p.Key && propertyValue == p.Value));

        return policies.ToList();
    }

    public async Task<Policy> UpdatePolicy(Policy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var policyEntity = context.Policies.Update(policy);
        await context.SaveChangesAsync();
        return policyEntity.Entity;
    }

    public async Task<bool> DeletePolicy(string policyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var policyEntity = await context.Policies
            .SingleOrDefaultAsync(p => p.PolicyId == policyId);

        if (policyId == null) return false;

        context.Remove(policyEntity!);
        await context.SaveChangesAsync();
        return true;
    }
}