using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class AuthorizationRegistryExtensions
{
    public static Organization DeepCopy(this Organization organization)
    {
        var newOrganization = new Organization(
            organization.Identifier,
            organization.Name,
            organization.Url,
            organization.Representative,
            organization.InvoicingContact);

        newOrganization.Employees = organization.Employees.Select(e => e.DeepCopy(newOrganization)).ToList();
        newOrganization.Properties = organization.Properties.Select(p => p.DeepCopy(newOrganization)).ToList();

        return newOrganization;
    }

    private static Organization.OrganizationProperty DeepCopy(this Organization.OrganizationProperty property, Organization newOrganization)
    {
        return new Organization.OrganizationProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            OrganizationId = property.OrganizationId,
            Organization = newOrganization
        };
    }

    public static Employee DeepCopy(this Employee employee, Organization newOrganization)
    {
        var newEmployee = new Employee(
            employee.EmployeeId,
            employee.GivenName,
            employee.FamilyName,
            employee.Telephone,
            employee.Email)
        {
            OrganizationId = employee.OrganizationId,
            Organization = newOrganization
        };

        newEmployee.Properties = employee.Properties.Select(p => p.DeepCopy(newEmployee)).ToList();

        return newEmployee;
    }

    private static Employee.EmployeeProperty DeepCopy(this Employee.EmployeeProperty property, Employee newEmployee)
    {
        return new Employee.EmployeeProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            EmployeeId = property.EmployeeId,
            Employee = newEmployee
        };
    }

    public static Policy DeepCopy(this Policy policy)
    {
        var newPolicy = new Policy(
            policy.UseCase,
            policy.IssuerId,
            policy.SubjectId,
            policy.ResourceId,
            policy.Action)
        {
            PolicyId = policy.PolicyId,
            IssuedAt = policy.IssuedAt,
            NotBefore = policy.NotBefore,
            Expiration = policy.Expiration
        };

        newPolicy.Properties = policy.Properties.Select(p => p.DeepCopy(newPolicy)).ToList();

        return newPolicy;
    }

    private static Policy.PolicyProperty DeepCopy(this Policy.PolicyProperty property, Policy newPolicy)
    {
        return new Policy.PolicyProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PolicyId = property.PolicyId,
            Policy = newPolicy
        };
    }

    public static Product DeepCopy(this Product product)
    {
        var newProduct = new Product(
            product.ProductId,
            product.Name,
            product.Description,
            product.Provider,
            product.Url);

        newProduct.Features = product.Features.Select(f => f.DeepCopy(newProduct)).ToList();
        newProduct.Properties = product.Properties.Select(p => p.DeepCopy(newProduct)).ToList();

        return newProduct;
    }

    private static Product DeepCopy(this Product product, Feature newFeature)
    {
        var newProduct = new Product(
            product.ProductId,
            product.Name,
            product.Description,
            product.Provider,
            product.Url);

        var featureList = product.Features.Select(f => f).ToList();
        featureList.RemoveAll(f => f.FeatureId.Equals(newFeature.FeatureId, StringComparison.OrdinalIgnoreCase));
        featureList.Add(newFeature);

        newProduct.Features = featureList;
        newProduct.Properties = product.Properties.Select(p => p.DeepCopy(newProduct)).ToList();

        return newProduct;
    }

    private static Product.ProductProperty DeepCopy(this Product.ProductProperty property, Product newProduct)
    {
        return new Product.ProductProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            ProductId = property.ProductId,
            Product = newProduct
        };
    }

    public static Feature DeepCopy(this Feature feature)
    {
        var newFeature = new Feature(
            feature.FeatureId,
            feature.Name,
            feature.Description);

        newFeature.Products = feature.Products.Select(p => p.DeepCopy(newFeature)).ToList();
        newFeature.Properties = feature.Properties.Select(p => p.DeepCopy(newFeature)).ToList();

        return newFeature;
    }

    private static Feature DeepCopy(this Feature feature, Product newProduct)
    {
        var newFeature = new Feature(
            feature.FeatureId,
            feature.Name,
            feature.Description);

        var productList = feature.Products.Select(p => p).ToList();
        productList.RemoveAll(p => p.ProductId.Equals(newProduct.ProductId, StringComparison.OrdinalIgnoreCase));
        productList.Add(newProduct);

        newFeature.Products = productList;
        newFeature.Properties = feature.Properties.Select(p => p.DeepCopy(newFeature)).ToList();

        return newFeature;
    }

    private static Feature.FeatureProperty DeepCopy(this Feature.FeatureProperty property, Feature newFeature)
    {
        return new Feature.FeatureProperty(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            FeatureId = property.FeatureId,
            Feature = newFeature
        };
    }
}