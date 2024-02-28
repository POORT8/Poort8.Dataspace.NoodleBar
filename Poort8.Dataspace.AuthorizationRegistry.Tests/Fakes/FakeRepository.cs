using Force.DeepCloner;
using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Policy = Poort8.Dataspace.AuthorizationRegistry.Entities.Policy;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests.Fakes;

public class FakeRepository : IRepository
{
    private readonly List<Organization> _organizations = new();
    private readonly List<Product> _products = new();
    private readonly List<Feature> _features = new();
    private readonly List<Policy> _policies = new();

    public Task<Policy> CreatePolicy(Policy policy)
    {
        if (policy.Action == "exception") throw new Exception("exception");

        _policies.Add(policy);
        return Task.FromResult(policy.DeepClone());
    }

    public Task<Policy?> ReadPolicy(string policyId)
    {
        return Task.FromResult(_policies.FirstOrDefault(p => p.PolicyId == policyId));
    }

    public async Task<Policy> UpdatePolicy(Policy policy)
    {
        _policies.RemoveAll(p => p.PolicyId == policy.PolicyId);
        var updatedPolicy = await CreatePolicy(policy);
        return updatedPolicy;
    }

    public Task<bool> DeletePolicy(string policyId)
    {
        _policies.RemoveAll(p => p.PolicyId == policyId);
        return Task.FromResult(true);
    }

    public Task<Employee> AddNewEmployeeToOrganization(string organizationId, Employee employee)
    {
        _organizations.First(o => o.Identifier == organizationId).Employees.Add(employee);
        return Task.FromResult(employee.DeepClone());
    }

    public Task<Organization> CreateOrganization(Organization organization)
    {
        _organizations.Add(organization);
        return Task.FromResult(organization.DeepClone());
    }

    public Task<IReadOnlyList<Organization>> ReadOrganizations(string? name, string? propertyKey, string? propertyValue)
    {
        var organizations = _organizations.Select(o => o.DeepClone()).ToList() as IReadOnlyList<Organization>;
        return Task.FromResult(organizations);
    }

    public Task<bool> DeleteEmployee(string employeeId)
    {
        foreach (var organization in _organizations)
        {
            foreach (var employee in organization.Employees)
            {
                if (employee.EmployeeId == employeeId)
                {
                    organization.Employees.Remove(employee);
                    return Task.FromResult(true);
                }
            }
        }
        return Task.FromResult(false);
    }

    public Task<Organization?> ReadOrganization(string identifier)
    {
        throw new NotImplementedException();
    }

    public Task<Organization> UpdateOrganization(Organization organization)
    {
        _organizations.RemoveAll(o => o.Identifier == organization.Identifier);
        _organizations.Add(organization);
        return Task.FromResult(organization.DeepClone());
    }

    public Task<bool> DeleteOrganization(string identifier)
    {
        throw new NotImplementedException();
    }

    public Task<Employee?> ReadEmployee(string employeeId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Employee>> ReadEmployees(string? organizationId = null, string? familyName = null, string? email = null, string? propertyKey = null, string? propertyValue = null)
    {
        throw new NotImplementedException();
    }

    public Task<Employee> UpdateEmployee(Employee employee)
    {
        foreach (var organization in _organizations)
        {
            foreach (var emp in organization.Employees)
            {
                if (emp.EmployeeId == employee.EmployeeId)
                {
                    organization.Employees.Remove(emp);
                    organization.Employees.Add(employee);
                    break;
                }
            }
        }
        return Task.FromResult(employee.DeepClone());
    }

    public Task<Product> CreateProduct(Product product)
    {
        _products.Add(product);
        return Task.FromResult(product.DeepClone());
    }

    public Task<Product> CreateProductWithExistingFeatures(Product product, ICollection<string> featureIds)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> ReadProduct(string productId)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.ProductId == productId));
    }

    public Task<IReadOnlyList<Product>> ReadProducts(string? name = null, string? propertyKey = null, string? propertyValue = null)
    {
        var products = _products.Select(o => o.DeepClone()).ToList() as IReadOnlyList<Product>;
        return Task.FromResult(products);
    }

    public Task<Product> UpdateProduct(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteProduct(string productId)
    {
        throw new NotImplementedException();
    }

    public Task<Feature> CreateFeature(Feature feature)
    {
        _features.Add(feature);
        return Task.FromResult(feature.DeepClone());
    }

    public Task<Feature> AddExistingFeatureToProduct(string productId, string featureId)
    {
        _products.First(o => o.ProductId == productId).Features.Add(_features.First(f => f.FeatureId == featureId));
        return Task.FromResult(_features.First(f => f.FeatureId == featureId).DeepClone());
    }

    public Task<Feature> AddNewFeatureToProduct(string productId, Feature feature)
    {
        _products.First(o => o.ProductId == productId).Features.Add(feature);
        return Task.FromResult(feature.DeepClone());
    }

    public Task<Feature?> ReadFeature(string featureId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Feature>> ReadFeatures(string? name = null, string? propertyKey = null, string? propertyValue = null)
    {
        throw new NotImplementedException();
    }

    public Task<Feature> UpdateFeature(Feature feature)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFeature(string featureId)
    {
        _features.RemoveAll(f => f.FeatureId == featureId);
        foreach (var product in _products)
        {
            foreach (var feature in product.Features)
            {
                if (feature.FeatureId == featureId)
                {
                    product.Features.Remove(feature);
                    return Task.FromResult(true);
                }
            }
        }
        return Task.FromResult(false);
    }

    public Task<bool> RemoveFeatureFromProduct(string productId, string featureId)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Policy>> ReadPolicies(string? useCase = null, string? issuerId = null, string? subjectId = null, string? resourceId = null, string? action = null, string? propertyKey = null, string? propertyValue = null)
    {
        var policies = _policies.Select(o => o.DeepClone()).ToList() as IReadOnlyList<Policy>;
        return Task.FromResult(policies);
    }

    public Task<IReadOnlyList<EntityAuditRecord>> ReadEntityAuditRecords(int numberOfRecords = 100)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<EnforceAuditRecord>> ReadEnforceAuditRecords(int numberOfRecords = 100)
    {
        throw new NotImplementedException();
    }

    public Task<EnforceAuditRecord> CreateEnforceAuditRecord(string user, string useCase, string subjectId, string resourceId, string action, bool allow, List<Policy>? explains = null)
    {
        return Task.FromResult(new EnforceAuditRecord(user, useCase, subjectId, resourceId, action, allow, explains));
    }
}