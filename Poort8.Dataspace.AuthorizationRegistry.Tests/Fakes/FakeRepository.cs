using Force.DeepCloner;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Policy = Poort8.Dataspace.AuthorizationRegistry.Entities.Policy;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests.Fakes;

public class FakeRepository : IRepository
{
    private readonly List<Organization> _organizations = new();
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
}