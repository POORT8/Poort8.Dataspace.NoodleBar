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
            organization.InvoicingContact,
            organization.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList());

        newOrganization.Employees = organization.Employees.Select(e => new Employee(
            e.EmployeeId,
            e.GivenName,
            e.FamilyName,
            e.Telephone,
            e.Email,
            e.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            OrganizationId = newOrganization.Identifier,
            Organization = newOrganization
        }).ToList();

        return newOrganization;
    }

    public static Employee DeepCopy(this Employee employee)
    {
        return new Employee(
            employee.EmployeeId,
            employee.GivenName,
            employee.FamilyName,
            employee.Telephone,
            employee.Email,
            employee.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            OrganizationId = employee.OrganizationId,
            Organization = employee.Organization
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
            policy.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            PolicyId = policy.PolicyId,
            IssuedAt = policy.IssuedAt,
            NotBefore = policy.NotBefore,
            Expiration = policy.Expiration
        };
    }

    public static Product DeepCopy(this Product product)
    {
        return new(
            product.ProductId,
            product.Name,
            product.Description,
            product.Provider,
            product.Url,
            product.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            Features = product.Features.Select(f => f).ToList()
        };
    }

    public static Feature DeepCopy(this Feature feature)
    {
        return new Feature(
            feature.FeatureId,
            feature.Name,
            feature.Description,
            feature.Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            Products = feature.Products.Select(p => p).ToList()
        };
    }
}