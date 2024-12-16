using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.AROrganizations.GetOrganizationById;

public class Mapper : ResponseMapper<Response, Organization>
{
    public override Response FromEntity(Organization organization)
    {
        return new Response(
            organization.Identifier,
            organization.UseCase,
            organization.Name,
            organization.Url,
            organization.Representative,
            organization.InvoicingContact,
            organization.Employees.Select(e => new EmployeeDto(
                e.EmployeeId,
                e.UseCase,
                e.GivenName,
                e.FamilyName,
                e.Telephone,
                e.Email,
                e.Properties.Select(p => new EmployeePropertyDto(
                    p.Key,
                    p.Value,
                    p.IsIdentifier)).ToList())).ToList(),
            organization.Properties.Select(p => new OrganizationPropertyDto(
                p.Key,
                p.Value,
                p.IsIdentifier)).ToList()
        );
    }
}
