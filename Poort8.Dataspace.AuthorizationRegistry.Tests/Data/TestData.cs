using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
internal static class TestData
{
    internal static Policy CreateNewPolicy() => new("issuer", "subject", "resource", "action");

    internal static Employee CreateNewEmployee(string id, int index)
    {
        var properties = new List<Employee.EmployeeProperty>() { new Employee.EmployeeProperty("key", "value"), new Employee.EmployeeProperty("otherIdentifier", $"{id}{index}-emp-otherId", true) };
        return new Employee($"{id}-emp-id", $"{id}{index}-emp-name", "familyName", "telephone", "email", properties);
    }

    internal static Resource CreateNewResource(string id, int index)
    {
        var properties = new List<Resource.ResourceProperty>() { new Resource.ResourceProperty("key", "value"), new Resource.ResourceProperty("otherIdentifier", $"{id}{index}-res-otherId", true) };
        return new Resource($"{id}-res-id", $"{id}{index}-res-name", "description", properties);
    }

    internal static Organization CreateNewOrganization(string id, int index)
    {
        var properties = new List<Organization.OrganizationProperty>() { new Organization.OrganizationProperty("key", "value"), new Organization.OrganizationProperty("otherIdentifier", $"{id}{index}-org-otherId", true) };
        return new Organization($"{id}-org-id", $"{id}{index}-org-name", "url", "representative", "invoicingContact", properties);
    }

    internal static ResourceGroup CreateNewResourceGroup(string id, int index)
    {
        var properties = new List<ResourceGroup.ResourceGroupProperty>() { new ResourceGroup.ResourceGroupProperty("key", "value"), new ResourceGroup.ResourceGroupProperty("otherIdentifier", $"{id}{index}-rg-otherId", true) };
        return new ResourceGroup($"{id}-rg-id", $"{id}{index}-rg-name", "description", "provider", "url", properties);
    }
}
