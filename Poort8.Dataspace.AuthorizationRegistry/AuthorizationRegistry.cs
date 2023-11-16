using Casbin;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.AuthorizationRegistry.Exceptions;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationRegistry : IAuthorizationRegistry
{
    private readonly IRepository _repository;
    private readonly IEnforcer _enforcer;

    public AuthorizationRegistry(IRepository repository)
    {
        _repository = repository;
        _enforcer = new Enforcer(EnforcerModel.Create());
    }

    #region Create

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        var organizationEntity = await _repository.CreateOrganization(organization);
        if (organization.Employees.Any()) await ResetSubjectGroup();
        return organizationEntity;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        var productEntity = await _repository.CreateProduct(product);
        if (product.Features.Any()) await ResetResourceGroup();
        return productEntity;
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
        var success = await _enforcer.AddPolicyAsync(policy.ToPolicyValues());
        if (!success) throw new EnforcerException("Could not add policy to enforcer.");

        try
        {
            return await _repository.CreatePolicy(policy);
        }
        catch (Exception)
        {
            success = await _enforcer.RemovePolicyAsync(policy.ToPolicyValues());
            if (!success) throw new EnforcerException("_repository.Create failed and could not remove policy from enforcer.");

            throw;
        }
    }

    public async Task<Employee> AddNewEmployeeToOrganization(string organizationId, Employee employee)
    {
        var employeeEntity = await _repository.AddNewEmployeeToOrganization(organizationId, employee);
        await ResetSubjectGroup();
        return employeeEntity;
    }

    public async Task<Feature> CreateFeature(Feature feature)
    {
        return await _repository.CreateFeature(feature);
    }

    public async Task<Feature> AddExistingFeatureToProduct(string productId, string featureId)
    {
        var featureEntity = await _repository.AddExistingFeatureToProduct(productId, featureId);
        await ResetResourceGroup();
        return featureEntity;
    }

    public async Task<Feature> AddNewFeatureToProduct(string productId, Feature feature)
    {
        var featureEntity = await _repository.AddNewFeatureToProduct(productId, feature);
        await ResetResourceGroup();
        return featureEntity;
    }

    #endregion
    #region Read

    public async Task<Organization?> ReadOrganization(string identifier)
    {
        return await _repository.ReadOrganization(identifier);
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadOrganizations(name, propertyKey, propertyValue);
    }

    public async Task<Employee?> ReadEmployee(string employeeId)
    {
        return await _repository.ReadEmployee(employeeId);
    }

    public async Task<IReadOnlyList<Employee>> ReadEmployees(
        string? organizationId = default,
        string? familyName = default,
        string? email = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadEmployees(organizationId, familyName, email, propertyKey, propertyValue);
    }

    public async Task<Product?> ReadProduct(string productId)
    {
        return await _repository.ReadProduct(productId);
    }

    public async Task<IReadOnlyList<Product>> ReadProducts(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadProducts(name, propertyKey, propertyValue);
    }

    public async Task<Feature?> ReadFeature(string featureId)
    {
        return await _repository.ReadFeature(featureId);
    }

    public async Task<IReadOnlyList<Feature>> ReadFeatures(
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadFeatures(name, propertyKey, propertyValue);
    }

    public async Task<Policy?> ReadPolicy(string policyId)
    {
        return await _repository.ReadPolicy(policyId);
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
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadPolicies(useCase, issuerId, subjectId, resourceId, action, propertyKey, propertyValue);
    }

    #endregion
    #region Update

    public async Task<Organization> UpdateOrganization(Organization organization)
    {
        var organizationEntity = await _repository.UpdateOrganization(organization);
        await ResetSubjectGroup();
        return organizationEntity;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        var employeeEntity = await _repository.UpdateEmployee(employee);
        await ResetSubjectGroup();
        return employeeEntity;
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        var productEntity = await _repository.UpdateProduct(product);
        await ResetResourceGroup();
        return productEntity;
    }

    public async Task<Feature> UpdateFeature(Feature feature)
    {
        var featureEntity = await _repository.UpdateFeature(feature);
        await ResetResourceGroup();
        return featureEntity;
    }

    public async Task<Policy> UpdatePolicy(Policy policy)
    {
        var oldPolicy = await ReadPolicy(policy.PolicyId) ?? throw new RepositoryException("Policy not found.");

        var success = await _enforcer.UpdateNamedPolicyAsync("p", oldPolicy.ToPolicyValues(), policy.ToPolicyValues());
        if (!success) throw new EnforcerException("Could not update policy in enforcer.");

        try
        {
            return await _repository.UpdatePolicy(policy);
        }
        catch (Exception)
        {
            success = await _enforcer.UpdateNamedPolicyAsync("p", policy.ToPolicyValues(), oldPolicy.ToPolicyValues());
            if (!success) throw new EnforcerException("_repository.Update failed and could not update policy in enforcer.");

            throw;
        }
    }

    #endregion
    #region Delete

    public async Task<bool> DeleteOrganization(string identifier)
    {
        //TODO: What to do with the policies?
        return await _repository.DeleteOrganization(identifier);
    }

    public async Task<bool> DeleteEmployee(string employeeId)
    {
        var success = await _repository.DeleteEmployee(employeeId);
        if (success) await ResetSubjectGroup();
        return success;
    }

    public async Task<bool> DeleteProduct(string productId)
    {
        //TODO: What to do with the policies?
        return await _repository.DeleteProduct(productId);
    }

    public async Task<bool> DeleteFeature(string featureId)
    {
        var success = await _repository.DeleteFeature(featureId);
        if (success) await ResetResourceGroup();
        return success;
    }

    public async Task<bool> RemoveFeatureFromProduct(string productId, string featureId)
    {
        return await _repository.RemoveFeatureFromProduct(productId, featureId);
    }

    public async Task<bool> DeletePolicy(string policyId)
    {
        var policyEntity = await ReadPolicy(policyId) ?? throw new RepositoryException("Policy not found.");

        var success = await _enforcer.RemovePolicyAsync(policyEntity.ToPolicyValues());
        if (!success) throw new EnforcerException("Could not delete policy from enforcer.");

        try
        {
            return await _repository.DeletePolicy(policyEntity.PolicyId);
        }
        catch (Exception)
        {
            success = await _enforcer.AddPolicyAsync(policyEntity.ToPolicyValues());
            if (!success) throw new EnforcerException("_repository.Delete failed and could not add policy to enforcer.");

            throw;
        }
    }

    #endregion

    #region Authorization

    public async Task<bool> Enforce(string subjectId, string resourceId, string action, string useCase = "default")
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        return await _enforcer.EnforceAsync(useCase, now, subjectId, resourceId, action);
    }

    public async Task<(bool allowed, List<Policy> explainPolicy)> ExplainedEnforce(string subjectId, string resourceId, string action, string useCase = "default")
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var (allowed, explainCasbinPolicies) = await _enforcer.EnforceExAsync(useCase, now, subjectId, resourceId, action);

        var explainPolicies = new List<Policy>(explainCasbinPolicies.Count());
        foreach (var explainCasbinPolicy in explainCasbinPolicies)
        {
            var explainPolicy = await ReadPolicy(explainCasbinPolicy.First()) ?? throw new EnforcerException("Explain policy not found.");
            explainPolicies.Add(explainPolicy);
        }

        return (allowed, explainPolicies);
    }

    #endregion

    private async Task ResetGroup(string groupKey)
    {
        var currentGroups = _enforcer.GetNamedGroupingPolicy(groupKey);
        var success = await _enforcer.RemoveNamedGroupingPoliciesAsync(groupKey, currentGroups);
        if (!success) throw new EnforcerException($"Could not remove current {groupKey} from enforcer.");
    }

    private async Task ResetSubjectGroup()
    {
        await ResetGroup("subjectGroup");

        var organizationEntities = await ReadOrganizations();
        var newGroups = GetAllSubjectGroups(organizationEntities);
        if (newGroups.Any())
        {
            var success = await _enforcer.AddNamedGroupingPoliciesAsync("subjectGroup", newGroups);
            if (!success) throw new EnforcerException("Could not add new subject groups to enforcer.");
        }
    }

    private static List<List<string>> GetAllSubjectGroups(IReadOnlyList<Organization> organizationEntities)
    {
        var organizationPropertyIdentifiers = organizationEntities
            .SelectMany(org => org.Properties
            .Where(p => p.IsIdentifier)
            .Select(p => new List<string> { p.Value, org.Identifier }))
            .ToList();

        var employeeIdentifiers = organizationEntities
            .SelectMany(org => org.Employees, (o, e) => new List<string> { e.EmployeeId, o.Identifier })
            .ToList();

        var employeeEmail = organizationEntities
            .SelectMany(org => org.Employees
            .Select(e => new List<string> { e.Email, org.Identifier }))
            .ToList();

        var employeeTelephone = organizationEntities
            .SelectMany(org => org.Employees
            .Select(e => new List<string> { e.Telephone, org.Identifier }))
            .ToList();

        var employeePropertyIdentifiers = organizationEntities
            .SelectMany(org => org.Employees
            .SelectMany(emp => emp.Properties
            .Where(p => p.IsIdentifier)
            .Select(p => new List<string> { p.Value, org.Identifier })))
            .ToList();

        return organizationPropertyIdentifiers
            .Concat(employeeIdentifiers)
            .Concat(employeeEmail)
            .Concat(employeeTelephone)
            .Concat(employeePropertyIdentifiers)
            .ToList();
    }

    private async Task ResetResourceGroup()
    {
        await ResetGroup("resourceGroup");

        var productEntities = await ReadProducts();
        var newGroups = GetAllResourceGroups(productEntities);
        if (newGroups.Any())
        {
            var success = await _enforcer.AddNamedGroupingPoliciesAsync("resourceGroup", newGroups);
            if (!success) throw new EnforcerException("Could not add new resource groups to enforcer.");
        }
    }

    private static List<List<string>> GetAllResourceGroups(IReadOnlyList<Product> productEntities)
    {
        var productPropertyIdentifiers = productEntities
            .SelectMany(prod => prod.Properties
            .Where(prop => prop.IsIdentifier)
            .Select(p => new List<string> { p.Value, prod.ProductId }))
            .ToList();

        var featureIdentifiers = productEntities
            .SelectMany(prod => prod.Features, (prod, f) => new List<string> { f.FeatureId, prod.ProductId })
            .ToList();

        var featurePropertyIdentifiers = productEntities
            .SelectMany(prod => prod.Features
            .SelectMany(feat => feat.Properties
            .Where(p => p.IsIdentifier)
            .Select(p => new List<string> { p.Value, prod.ProductId })))
            .ToList();

        return productPropertyIdentifiers
            .Concat(featureIdentifiers)
            .Concat(featurePropertyIdentifiers)
            .ToList();
    }

    private static void ValidateReadQuery(string? propertyKey, string? propertyValue)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");
    }

    public async Task<IReadOnlyList<AuditRecord>> GetAuditRecords()
    {
        return await _repository.ReadAuditRecords();
    }
}