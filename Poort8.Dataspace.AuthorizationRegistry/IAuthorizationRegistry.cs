using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public interface IAuthorizationRegistry //TODO: Add authorizations for managing the registry itself
{
    //Organization
    Task<Organization> CreateOrganization(Organization organization);
    Task<Organization?> ReadOrganization(string identifier);
    Task<IReadOnlyList<Organization>> ReadOrganizations(string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Organization> UpdateOrganization(Organization organization);
    Task<bool> DeleteOrganization(string identifier);

    //Employee
    Task<Employee> AddEmployee(string organizationId, Employee employee);
    Task<Employee?> ReadEmployee(string employeeId);
    Task<IReadOnlyList<Employee>> ReadEmployees(string? organizationId = default, string? familyName = default, string? email = default, string? propertyKey = default, string? propertyValue = default);
    Task<Employee> UpdateEmployee(Employee employee);
    Task<bool> DeleteEmployee(string employeeId);

    //Product
    Task<Product> CreateProduct(Product product);
    Task<Product?> ReadProduct(string productId);
    Task<IReadOnlyList<Product>> ReadProducts(string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Product> UpdateProduct(Product product);
    Task<bool> DeleteProduct(string productId);

    //Feature
    Task<Feature> AddFeature(string productId, Feature feature);
    Task<Feature?> ReadFeature(string featureId);
    Task<IReadOnlyList<Feature>> ReadFeatures(string? name = default, string? propertyKey = default, string? propertyValue = default);
    Task<Feature> UpdateFeature(Feature feature);
    Task<bool> DeleteFeature(string featureId);

    //Policy
    Task<Policy> CreatePolicy(Policy policy);
    Task<Policy?> ReadPolicy(string policyId);
    Task<IReadOnlyList<Policy>> ReadPolicies(string? useCase = default, string? issuerId = default, string? subjectId = default, string? resourceId = default, string? action = default, string? propertyKey = default, string? propertyValue = default);
    Task<Policy> UpdatePolicy(Policy policy);
    Task<bool> DeletePolicy(string policyId);

    //TODO: Add enforce
}