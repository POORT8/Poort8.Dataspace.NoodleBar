using Force.DeepCloner;
using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Policy = Poort8.Dataspace.AuthorizationRegistry.Entities.Policy;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests.Fakes;

public class FakeRepository : IRepository
{
    private readonly List<Organization> _organizations = new();
    private readonly List<Employee> _employees = new();
    private readonly List<ResourceGroup> _resourceGroups = new();
    private readonly List<Resource> _resources = new();
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

    public Task<IReadOnlyList<Organization>> ReadOrganizations(string? useCase, string? name, string? propertyKey, string? propertyValue)
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
        return Task.FromResult(_organizations.FirstOrDefault(p => p.Identifier == identifier));
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
        return Task.FromResult(_employees.FirstOrDefault(p => p.EmployeeId == employeeId));
    }

    public Task<IReadOnlyList<Employee>> ReadEmployees(string? useCase, string? organizationId = null, string? familyName = null, string? email = null, string? propertyKey = null, string? propertyValue = null)
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

    public Task<ResourceGroup> CreateResourceGroup(ResourceGroup resourceGroup)
    {
        _resourceGroups.Add(resourceGroup);
        return Task.FromResult(resourceGroup.DeepClone());
    }

    public Task<ResourceGroup> CreateResourceGroupWithExistingResources(ResourceGroup resourceGroup, ICollection<string> resourceIds)
    {
        throw new NotImplementedException();
    }

    public Task<ResourceGroup?> ReadResourceGroup(string resourceGroupId)
    {
        return Task.FromResult(_resourceGroups.FirstOrDefault(p => p.ResourceGroupId == resourceGroupId));
    }

    public Task<IReadOnlyList<ResourceGroup>> ReadResourceGroups(string? useCase, string? name = null, string? propertyKey = null, string? propertyValue = null)
    {
        var resourceGroups = _resourceGroups.Select(o => o.DeepClone()).ToList() as IReadOnlyList<ResourceGroup>;
        return Task.FromResult(resourceGroups);
    }

    public Task<ResourceGroup> UpdateResourceGroup(ResourceGroup resourceGroup)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteResourceGroup(string resourceGroupId)
    {
        throw new NotImplementedException();
    }

    public Task<Resource> CreateResource(Resource resource)
    {
        _resources.Add(resource);
        return Task.FromResult(resource.DeepClone());
    }

    public Task<Resource> AddExistingResourceToResourceGroup(string resourceGroupId, string resourceId)
    {
        _resourceGroups.First(o => o.ResourceGroupId == resourceGroupId).Resources.Add(_resources.First(f => f.ResourceId == resourceId));
        return Task.FromResult(_resources.First(f => f.ResourceId == resourceId).DeepClone());
    }

    public Task<Resource> AddNewResourceToResourceGroup(string resourceGroupId, Resource resource)
    {
        _resourceGroups.First(o => o.ResourceGroupId == resourceGroupId).Resources.Add(resource);
        return Task.FromResult(resource.DeepClone());
    }

    public Task<Resource?> ReadResource(string resourceId)
    {
        return Task.FromResult(_resources.FirstOrDefault(p => p.ResourceId == resourceId));
    }

    public Task<IReadOnlyList<Resource>> ReadResources(string? useCase, string? name = null, string? propertyKey = null, string? propertyValue = null)
    {
        throw new NotImplementedException();
    }

    public Task<Resource> UpdateResource(Resource resource)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteResource(string resourceId)
    {
        _resources.RemoveAll(f => f.ResourceId == resourceId);
        foreach (var resourceGroup in _resourceGroups)
        {
            foreach (var resource in resourceGroup.Resources)
            {
                if (resource.ResourceId == resourceId)
                {
                    resourceGroup.Resources.Remove(resource);
                    return Task.FromResult(true);
                }
            }
        }
        return Task.FromResult(false);
    }

    public Task<bool> RemoveResourceFromResourceGroup(string resourceGroupId, string resourceId)
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

    public Task<EnforceAuditRecord> CreateEnforceAuditRecord(string user, string useCase, string subjectId, string resourceId, string action, bool allow, List<Policy>? explains, string? issuerId, string? serviceProvider, string? type, string? attribute, string? requestContext)
    {
        return Task.FromResult(new EnforceAuditRecord(user, useCase, subjectId, resourceId, action, allow, explains));
    }
}