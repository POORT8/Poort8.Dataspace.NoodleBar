using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public interface IAuthorizationRegistry
{
    //Organization
    Task<Organization> CreateOrganization(Organization organization);
    Task<Organization?> ReadOrganization(string identifier);
    Task<IReadOnlyList<Organization>> ReadOrganizations(string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Organization> UpdateOrganization(Organization organization);
    Task<bool> DeleteOrganization(string identifier);

    //Employee
    Task<Employee> CreateEmployee(Employee employee);
    Task<Employee?> ReadEmployee(string employeeId);
    Task<IReadOnlyList<Employee>> ReadEmployees(string? organizationId = default, string? familyName = default, string? email = default, string? propertyKey = default, string? propertyValue = default);
    Task<Employee> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(string employeeId);

    //Action
    Task<Entities.Action> CreateAction(Entities.Action action);
    Task<Entities.Action?> ReadAction(string actionId);
    Task<IReadOnlyList<Entities.Action>> ReadActions(string? name = default, string? additionalType = default);
    Task<Entities.Action> UpdateAction(Entities.Action action);
    Task<bool> DeleteAction(string actionId);

    //Product
    Task<Product> CreateProduct(Product product);
    Task<Product?> ReadProduct(string productId);
    Task<IReadOnlyList<Product>> ReadProducts(string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Product> UpdateProduct(Product product);
    Task<bool> DeleteProduct(string productId);

    //Service
    Task<Service> CreateService(Service service);
    Task<Service?> ReadService(string serviceId);
    Task<IReadOnlyList<Service>> ReadServices(string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Service> UpdateService(Service service);
    Task<bool> DeleteService(string serviceId);

    //Policy
    Task<Policy> CreatePolicy(Policy policy);
    Task<Policy?> ReadPolicy(string policyId);
    Task<IReadOnlyList<Policy>> ReadPolicies(string? issuer = default, string? subject = default, string? resource = default, string? action = default, string? propertyKey = default, string? propertyValue = default);
    Task<Policy> UpdatePolicy(Policy policy);
    Task<bool> DeletePolicy(string policyId);
}