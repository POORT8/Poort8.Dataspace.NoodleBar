using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public interface IRepository
{
    Task<Policy> CreatePolicy(Policy policy);
    Task<Organization> CreateOrganization(Organization organization);
    Task<Policy?> ReadPolicy(string policyId);
    Task<Policy> UpdatePolicy(Policy policy);
    Task<bool> DeletePolicy(string policyId);
    Task<Employee> AddNewEmployeeToOrganization(string organizationId, Employee employee);
    Task<IReadOnlyList<Organization>> ReadOrganizations(string? name, string? propertyKey, string? propertyValue);
    Task<bool> DeleteEmployee(string employeeId);
}