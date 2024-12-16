using Casbin;
using Microsoft.AspNetCore.Http;
using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.AuthorizationRegistry.Exceptions;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using System.Text.Json;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class AuthorizationRegistry : IAuthorizationRegistry
{
    private readonly IRepository _repository;
    private readonly IEnforcer _enforcer;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    //TODO: Add tests for group reset in all cases.

    public AuthorizationRegistry(IRepository repository, IHttpContextAccessor? httpContextAccessor = null)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;

        _enforcer = new Enforcer(EnforcerModel.Create());

        LoadAllPoliciesAndGroupsFromStorage();
    }

    private async void LoadAllPoliciesAndGroupsFromStorage()
    {
        var policies = await _repository.ReadPolicies();

        foreach (var policy in policies)
        {
            string policyType = UseCases.GetPolicyType(policy);

            var success = await _enforcer.AddNamedPolicyAsync(policyType, policy.ToPolicyValues());
            if (!success) throw new EnforcerException("Could not add policy to enforcer.");
        }

        await ResetSubjectGroup();
        await ResetResourceGroup();
    }

    #region Create

    public async Task<Organization> CreateOrganization(Organization organization)
    {
        var existingEntity = await ReadOrganization(organization.Identifier);
        if (existingEntity != null)
            throw new RepositoryException($"{RepositoryException.IdNotUnique}: Organization with identifier {organization.Identifier} already exists.");

        var organizationEntity = await _repository.CreateOrganization(organization);
        await ResetSubjectGroup();
        return organizationEntity;
    }

    public async Task<ResourceGroup> CreateResourceGroup(ResourceGroup resourceGroup)
    {
        var existingEntity = await ReadResourceGroup(resourceGroup.ResourceGroupId);
        if (existingEntity != null)
            throw new RepositoryException($"{RepositoryException.IdNotUnique}: ResourceGroup with id {resourceGroup.ResourceGroupId} already exists.");

        var resourceGroupEntity = await _repository.CreateResourceGroup(resourceGroup);
        await ResetResourceGroup();
        return resourceGroupEntity;
    }

    public async Task<ResourceGroup> CreateResourceGroupWithExistingResources(ResourceGroup resourceGroup, ICollection<string> resourceIds)
    {
        var existingEntity = await ReadResourceGroup(resourceGroup.ResourceGroupId);
        if (existingEntity != null)
            throw new RepositoryException($"{RepositoryException.IdNotUnique}: ResourceGroup with id {resourceGroup.ResourceGroupId} already exists.");

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
        string policyType = UseCases.GetPolicyType(policy);

        var success = await _enforcer.AddNamedPolicyAsync(policyType, policy.ToPolicyValues());
        if (!success) throw new EnforcerException("Could not add policy to enforcer.");

        try
        {
            return await _repository.CreatePolicy(policy);
        }
        catch (Exception)
        {
            success = await _enforcer.RemoveNamedPolicyAsync(policyType, policy.ToPolicyValues());
            if (!success) throw new EnforcerException("_repository.Create failed and could not remove policy from enforcer.");

            throw;
        }
    }

    public async Task<Employee> AddNewEmployeeToOrganization(string organizationId, Employee employee)
    {
        var existingEntity = await ReadEmployee(employee.EmployeeId);
        if (existingEntity != null)
            throw new RepositoryException($"{RepositoryException.IdNotUnique}: Employee with id {employee.EmployeeId} already exists.");

        var employeeEntity = await _repository.AddNewEmployeeToOrganization(organizationId, employee);
        await ResetSubjectGroup();
        return employeeEntity;
    }

    public async Task<Resource> CreateResource(Resource resource)
    {
        var existingEntity = await ReadResource(resource.ResourceId);
        if (existingEntity != null)
            throw new RepositoryException($"{RepositoryException.IdNotUnique}: Resource with id {resource.ResourceId} already exists.");

        var resourceEntity = await _repository.CreateResource(resource);
        await ResetResourceGroup();
        return resourceEntity;
    }

    public async Task<Resource> AddExistingResourceToResourceGroup(string resourceGroupId, string resourceId)
    {
        var resourceEntity = await _repository.AddExistingResourceToResourceGroup(resourceGroupId, resourceId);
        await ResetResourceGroup();
        return resourceEntity;
    }

    public async Task<Resource> AddNewResourceToResourceGroup(string resourceGroupId, Resource resource)
    {
        var existingEntity = await ReadResource(resource.ResourceId);
        if (existingEntity != null)
            throw new RepositoryException($"{RepositoryException.IdNotUnique}: Resource with id {resource.ResourceId} already exists.");

        var resourceEntity = await _repository.AddNewResourceToResourceGroup(resourceGroupId, resource);
        await ResetResourceGroup();
        return resourceEntity;
    }

    #endregion
    #region Read

    public async Task<Organization?> ReadOrganization(string identifier)
    {
        return await _repository.ReadOrganization(identifier);
    }

    public async Task<IReadOnlyList<Organization>> ReadOrganizations(
        string? useCase = default,
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadOrganizations(useCase, name, propertyKey, propertyValue);
    }

    public async Task<Employee?> ReadEmployee(string employeeId)
    {
        return await _repository.ReadEmployee(employeeId);
    }

    public async Task<IReadOnlyList<Employee>> ReadEmployees(
        string? useCase = default,
        string? organizationId = default,
        string? familyName = default,
        string? email = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadEmployees(useCase, organizationId, familyName, email, propertyKey, propertyValue);
    }

    public async Task<ResourceGroup?> ReadResourceGroup(string resourceGroupId)
    {
        return await _repository.ReadResourceGroup(resourceGroupId);
    }

    public async Task<IReadOnlyList<ResourceGroup>> ReadResourceGroups(
        string? useCase = default,
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadResourceGroups(useCase, name, propertyKey, propertyValue);
    }

    public async Task<Resource?> ReadResource(string resourceId)
    {
        return await _repository.ReadResource(resourceId);
    }

    public async Task<IReadOnlyList<Resource>> ReadResources(
        string? useCase = default,
        string? name = default,
        string? propertyKey = default,
        string? propertyValue = default)
    {
        ValidateReadQuery(propertyKey, propertyValue);
        return await _repository.ReadResources(useCase, name, propertyKey, propertyValue);
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

    public async Task<ResourceGroup> UpdateResourceGroup(ResourceGroup resourceGroup)
    {
        var resourceGroupEntity = await _repository.UpdateResourceGroup(resourceGroup);
        await ResetResourceGroup();
        return resourceGroupEntity;
    }

    public async Task<Resource> UpdateResource(Resource resource)
    {
        var resourceEntity = await _repository.UpdateResource(resource);
        await ResetResourceGroup();
        return resourceEntity;
    }

    public async Task<Policy> UpdatePolicy(Policy policy)
    {
        var oldPolicy = await ReadPolicy(policy.PolicyId) ?? throw new RepositoryException("Policy not found.");

        string policyType = UseCases.GetPolicyType(policy);

        var success = await _enforcer.UpdateNamedPolicyAsync(policyType, oldPolicy.ToPolicyValues(), policy.ToPolicyValues());
        if (!success) throw new EnforcerException("Could not update policy in enforcer.");

        try
        {
            return await _repository.UpdatePolicy(policy);
        }
        catch (Exception)
        {
            success = await _enforcer.UpdateNamedPolicyAsync(policyType, policy.ToPolicyValues(), oldPolicy.ToPolicyValues());
            if (!success) throw new EnforcerException("_repository.Update failed and could not update policy in enforcer.");

            throw;
        }
    }

    #endregion
    #region Delete

    public async Task<bool> DeleteOrganization(string identifier)
    {
        //TODO: What to do with the policies?
        var success = await _repository.DeleteOrganization(identifier);
        if (success) await ResetSubjectGroup();
        return success;
    }

    public async Task<bool> DeleteEmployee(string employeeId)
    {
        var success = await _repository.DeleteEmployee(employeeId);
        if (success) await ResetSubjectGroup();
        return success;
    }

    public async Task<bool> DeleteResourceGroup(string resourceGroupId)
    {
        //TODO: What to do with the policies?
        var success = await _repository.DeleteResourceGroup(resourceGroupId);
        if (success) await ResetResourceGroup();
        return success;
    }

    public async Task<bool> DeleteResource(string resourceId)
    {
        var success = await _repository.DeleteResource(resourceId);
        if (success) await ResetResourceGroup();
        return success;
    }

    public async Task<bool> RemoveResourceFromResourceGroup(string resourceGroupId, string resourceId)
    {
        var success = await _repository.RemoveResourceFromResourceGroup(resourceGroupId, resourceId);
        if (success) await ResetResourceGroup();
        return success;
    }

    public async Task<bool> DeletePolicy(string policyId)
    {
        var policyEntity = await ReadPolicy(policyId) ?? throw new RepositoryException("Policy not found.");

        string policyType = UseCases.GetPolicyType(policyEntity);

        var success = await _enforcer.RemoveNamedPolicyAsync(policyType, policyEntity.ToPolicyValues());
        if (!success) throw new EnforcerException("Could not delete policy from enforcer.");

        try
        {
            return await _repository.DeletePolicy(policyEntity.PolicyId);
        }
        catch (Exception)
        {
            success = await _enforcer.AddNamedPolicyAsync(policyType, policyEntity.ToPolicyValues());
            if (!success) throw new EnforcerException("_repository.Delete failed and could not add policy to enforcer.");

            throw;
        }
    }

    #endregion
    #region Authorization

    public async Task<bool> Enforce(string subjectId, string resourceId, string action, string useCase = "default")
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var allowed = await _enforcer.EnforceAsync(useCase.ToLower(), now, subjectId.ToLower(), resourceId.ToLower(), action.ToLower());

        var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "unknown";
        await _repository.CreateEnforceAuditRecord(user, useCase, subjectId, resourceId, action, allowed, null, null, null, null, null, null);

        return allowed;
    }

    public async Task<(bool allowed, List<Policy> explainPolicy)> ExplainedEnforce(string subjectId, string resourceId, string action, string useCase = "default")
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var (allowed, explainCasbinPolicies) = await _enforcer.EnforceExAsync(useCase.ToLower(), now, subjectId.ToLower(), resourceId.ToLower(), action.ToLower());

        var explainPolicies = new List<Policy>(explainCasbinPolicies.Count());
        foreach (var explainCasbinPolicy in explainCasbinPolicies)
        {
            var explainPolicy = await ReadPolicy(explainCasbinPolicy.First()) ?? throw new EnforcerException("Explain policy not found.");
            explainPolicies.Add(explainPolicy);
        }

        var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "unknown";
        await _repository.CreateEnforceAuditRecord(user, useCase, subjectId, resourceId, action, allowed, explainPolicies, null, null, null, null, null);

        return (allowed, explainPolicies);
    }

    public async Task<(bool allowed, List<Policy> explainPolicy)> ExplainedEnforce(string issuerId, string subjectId, string serviceProvider, string action, string resourceId, string type, string attribute, string useCase = "ishare")
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var context = _enforcer.CreateContext("rishare", "pishare", "e", "mishare", true);
        var (allowed, explainCasbinPolicies) = await AREnforcerExtension.EnforceExAsync(_enforcer, context, useCase.ToLower(), now, issuerId.ToLower(), subjectId.ToLower(), serviceProvider.ToLower(), action.ToLower(), resourceId.ToLower(), type.ToLower(), attribute.ToLower());

        var explainPolicies = new List<Policy>(explainCasbinPolicies.Count());
        foreach (var explainCasbinPolicy in explainCasbinPolicies)
        {
            var explainPolicy = await ReadPolicy(explainCasbinPolicy.First()) ?? throw new EnforcerException("Explain policy not found.");
            explainPolicies.Add(explainPolicy);
        }

        var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "unknown";
        await _repository.CreateEnforceAuditRecord(user, useCase, subjectId, resourceId, action, allowed, explainPolicies, issuerId, serviceProvider, type, attribute, null);

        return (allowed, explainPolicies);
    }

    public async Task<(bool allowed, List<Policy> explainPolicy)> ExplainedEnforce(string issuerId, string subjectId, string serviceProvider, string action, string resourceId, string type, string attribute, dynamic requestContext, string useCase = "isharerules")
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        _enforcer.AddFunction("EnforceIshareRules", RuleEnforcers.GetRuleEnforcer(useCase));
        var context = _enforcer.CreateContext("risharerules", "pisharerules", "e", "misharerules", true);
        var (allowed, explainCasbinPolicies) = await AREnforcerExtension.EnforceExAsync(_enforcer, context, useCase.ToLower(), now, issuerId.ToLower(), subjectId.ToLower(), serviceProvider.ToLower(), action.ToLower(), resourceId.ToLower(), type.ToLower(), attribute.ToLower(), requestContext as object);

        var explainPolicies = new List<Policy>(explainCasbinPolicies.Count());
        foreach (var explainCasbinPolicy in explainCasbinPolicies)
        {
            var explainPolicy = await ReadPolicy(explainCasbinPolicy.First()) ?? throw new EnforcerException("Explain policy not found.");
            explainPolicies.Add(explainPolicy);
        }

        var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "unknown";
        await _repository.CreateEnforceAuditRecord(user, useCase, subjectId, resourceId, action, allowed, explainPolicies, issuerId, serviceProvider, type, attribute, JsonSerializer.Serialize(requestContext));

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
        if (newGroups.Count > 0)
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
            .Select(p => new List<string> { p.Value.ToLower(), org.Identifier.ToLower(), org.UseCase.ToLower() }))
            .ToList();

        var employeeIdentifiers = organizationEntities
            .SelectMany(org => org.Employees, (o, e) => new List<string> { e.EmployeeId.ToLower(), o.Identifier.ToLower(), e.UseCase.ToLower() })
            .ToList();

        var employeeEmail = organizationEntities
            .SelectMany(org => org.Employees
            .Select(e => new List<string> { e.Email.ToLower(), org.Identifier.ToLower(), e.UseCase.ToLower() }))
            .ToList();

        var employeeTelephone = organizationEntities
            .SelectMany(org => org.Employees
            .Select(e => new List<string> { e.Telephone.ToLower(), org.Identifier.ToLower(), e.UseCase.ToLower() }))
            .ToList();

        var employeePropertyIdentifiers = organizationEntities
            .SelectMany(org => org.Employees
            .SelectMany(emp => emp.Properties
            .Where(p => p.IsIdentifier)
            .Select(p => new List<string> { p.Value.ToLower(), org.Identifier.ToLower(), emp.UseCase.ToLower() })))
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

        var resourceGroupEntities = await ReadResourceGroups();
        var newGroups = GetAllResourceGroups(resourceGroupEntities);
        if (newGroups.Count > 0)
        {
            var success = await _enforcer.AddNamedGroupingPoliciesAsync("resourceGroup", newGroups);
            if (!success) throw new EnforcerException("Could not add new resource groups to enforcer.");
        }
    }

    private static List<List<string>> GetAllResourceGroups(IReadOnlyList<ResourceGroup> resourceGroupEntities)
    {
        var resourceGroupPropertyIdentifiers = resourceGroupEntities
            .SelectMany(rg => rg.Properties
            .Where(rg => rg.IsIdentifier)
            .Select(p => new List<string> { p.Value.ToLower(), rg.ResourceGroupId.ToLower(), rg.UseCase.ToLower() }))
            .ToList();

        var resourceIdentifiers = resourceGroupEntities
            .SelectMany(rg => rg.Resources, (rg, f) => new List<string> { f.ResourceId.ToLower(), rg.ResourceGroupId.ToLower(), f.UseCase.ToLower() })
            .ToList();

        var resourcePropertyIdentifiers = resourceGroupEntities
            .SelectMany(rg => rg.Resources
            .SelectMany(r => r.Properties
            .Where(p => p.IsIdentifier)
            .Select(p => new List<string> { p.Value.ToLower(), rg.ResourceGroupId.ToLower(), r.UseCase.ToLower() })))
            .ToList();

        return resourceGroupPropertyIdentifiers
            .Concat(resourceIdentifiers)
            .Concat(resourcePropertyIdentifiers)
            .ToList();
    }

    private static void ValidateReadQuery(string? propertyKey, string? propertyValue)
    {
        if ((propertyKey != default && propertyValue == default) || (propertyKey == default && propertyValue != default))
            throw new ArgumentException("PropertyValue must be set when propertyKey is set.");
    }

    public async Task<IReadOnlyList<EntityAuditRecord>> GetEntityAuditRecords()
    {
        return await _repository.ReadEntityAuditRecords();
    }

    public async Task<IReadOnlyList<EnforceAuditRecord>> GetEnforceAuditRecords()
    {
        return await _repository.ReadEnforceAuditRecords();
    }
}