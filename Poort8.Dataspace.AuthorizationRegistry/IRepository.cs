using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public interface IRepository
{
    //Organization
    Task<Organization> CreateOrganization(Organization organization);
    Task<Organization?> ReadOrganization(string identifier);
    Task<IReadOnlyList<Organization>> ReadOrganizations(string? useCase = default, string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Organization> UpdateOrganization(Organization organization);
    Task<bool> DeleteOrganization(string identifier);

    //Employee
    Task<Employee> AddNewEmployeeToOrganization(string organizationId, Employee employee);
    Task<Employee?> ReadEmployee(string employeeId);
    Task<IReadOnlyList<Employee>> ReadEmployees(string? useCase = default, string? organizationId = default, string? familyName = default, string? email = default, string? propertyKey = default, string? propertyValue = default);
    Task<Employee> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(string employeeId);

    //ResourceGroup
    Task<ResourceGroup> CreateResourceGroup(ResourceGroup resourceGroup);
    Task<ResourceGroup> CreateResourceGroupWithExistingResources(ResourceGroup resourceGroup, ICollection<string> resourceIds);
    Task<ResourceGroup?> ReadResourceGroup(string resourceGroupId);
    Task<IReadOnlyList<ResourceGroup>> ReadResourceGroups(string? useCase = default, string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<ResourceGroup> UpdateResourceGroup(ResourceGroup resourceGroup);
    Task<bool> DeleteResourceGroup(string resourceGroupId);

    //Resource
    Task<Resource> CreateResource(Resource resource);
    Task<Resource> AddExistingResourceToResourceGroup(string resourceGroupId, string resourceId);
    Task<Resource> AddNewResourceToResourceGroup(string resourceGroupId, Resource resource);
    Task<Resource?> ReadResource(string resourceId);
    Task<IReadOnlyList<Resource>> ReadResources(string? useCase = default, string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Resource> UpdateResource(Resource resource);
    Task<bool> DeleteResource(string resourceId);
    Task<bool> RemoveResourceFromResourceGroup(string resourceGroupId, string resourceId);

    //Policy
    Task<Policy> CreatePolicy(Policy policy);
    Task<Policy?> ReadPolicy(string policyId);
    Task<IReadOnlyList<Policy>> ReadPolicies(string? useCase = default, string? issuerId = default, string? subjectId = default, string? resourceId = default, string? action = default, string? propertyKey = default, string? propertyValue = default);
    Task<Policy> UpdatePolicy(Policy policy);
    Task<bool> DeletePolicy(string policyId);

    //Audit
    Task<IReadOnlyList<EntityAuditRecord>> ReadEntityAuditRecords(int numberOfRecords = 100);
    Task<IReadOnlyList<EnforceAuditRecord>> ReadEnforceAuditRecords(int numberOfRecords = 100);
    Task<EnforceAuditRecord> CreateEnforceAuditRecord(string user, string useCase, string subjectId, string resourceId, string action, bool allow, List<Policy>? explains, string? issuerId, string? serviceProvider, string? type, string? attribute, string? requestContext);
}