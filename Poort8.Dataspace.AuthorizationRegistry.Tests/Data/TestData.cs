using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
internal static class TestData
{
    internal static Policy CreateNewPolicy() => new("issuer", "subject", "resource", "action");

    internal static Employee CreateNewEmployee(string id, int index)
    {
        var properties = new List<Employee.EmployeeProperty>() { new Employee.EmployeeProperty("key", "value"), new Employee.EmployeeProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Employee($"{id}-emp-id", $"{id}{index}-emp-name", "familyName", "telephone", "email", properties);
    }

    internal static Feature CreateNewFeature(string id, int index)
    {
        var properties = new List<Feature.FeatureProperty>() { new Feature.FeatureProperty("key", "value"), new Feature.FeatureProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Feature($"{id}-feat-id", $"{id}{index}-feat-name", "description", properties);
    }

    internal static Organization CreateNewOrganization(string id, int index)
    {
        var properties = new List<Organization.OrganizationProperty>() { new Organization.OrganizationProperty("key", "value"), new Organization.OrganizationProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Organization($"{id}-org-id", $"{id}{index}-org-name", "url", "representative", "invoicingContact", properties);
    }

    internal static Product CreateNewProduct(string id, int index)
    {
        var properties = new List<Product.ProductProperty>() { new Product.ProductProperty("key", "value"), new Product.ProductProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Product($"{id}-prod-id", $"{id}{index}-prod-name", "description", "provider", "url", properties);
    }
}
