using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class AuthorizationRegistryExtensions
{
    public static Organization DeepCopy(this Organization organization)
    {
        return new Organization(
            organization.Identifier,
            organization.Name,
            organization.Url,
            organization.Representative,
            organization.InvoicingContact,
            organization.Properties.Select(p => p.DeepCopy()).ToList())
        {
            Employees = organization.Employees.Select(e => e.DeepCopy()).ToList()
        };
    }

    private static Organization.OrganizationProperty DeepCopy(this Organization.OrganizationProperty property)
    {
        return new Organization.OrganizationProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }

    public static Employee DeepCopy(this Employee employee)
    {
        return new Employee(
            employee.EmployeeId,
            employee.GivenName,
            employee.FamilyName,
            employee.Telephone,
            employee.Email,
            employee.Properties.Select(p => p.DeepCopy()).ToList());
    }

    private static Employee.EmployeeProperty DeepCopy(this Employee.EmployeeProperty property)
    {
        return new Employee.EmployeeProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }

    public static Policy DeepCopy(this Policy policy)
    {
        return new Policy(
            policy.UseCase,
            policy.IssuerId,
            policy.SubjectId,
            policy.ResourceId,
            policy.Action,
            policy.Properties.Select(p => p.DeepCopy()).ToList())
        {
            PolicyId = policy.PolicyId,
            IssuedAt = policy.IssuedAt,
            NotBefore = policy.NotBefore,
            Expiration = policy.Expiration
        };
    }

    private static Policy.PolicyProperty DeepCopy(this Policy.PolicyProperty property)
    {
        return new Policy.PolicyProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }

    public static Product DeepCopy(this Product product)
    {
        return new Product(
            product.ProductId,
            product.Name,
            product.Description,
            product.Provider,
            product.Url,
            product.Properties.Select(p => p.DeepCopy()).ToList())
        {
            Features = product.Features.Select(f => f.DeepCopy()).ToList()
        };
    }

    private static Product.ProductProperty DeepCopy(this Product.ProductProperty property)
    {
        return new Product.ProductProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }

    public static Feature DeepCopy(this Feature feature)
    {
        return new Feature(
            feature.FeatureId,
            feature.Name,
            feature.Description,
            feature.Properties.Select(p => p.DeepCopy()).ToList());
    }

    private static Feature.FeatureProperty DeepCopy(this Feature.FeatureProperty property)
    {
        return new Feature.FeatureProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }
}