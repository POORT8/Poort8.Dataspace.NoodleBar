namespace Poort8.Dataspace.API.AROrganizations.GetOrganizationById;

public record Response(
    string Identifier,
    string UseCase,
    string Name,
    string Url,
    string Representative,
    string InvoicingContact,
    ICollection<EmployeeDto> Employees,
    ICollection<OrganizationPropertyDto> Properties);

public record EmployeeDto(
    string EmployeeId,
    string UseCase,
    string GivenName,
    string FamilyName,
    string Telephone,
    string Email,
    ICollection<EmployeePropertyDto> Properties);

public record EmployeePropertyDto(
    string Key,
    string Value,
    bool IsIdentifier);

public record OrganizationPropertyDto(
    string Key,
    string Value,
    bool IsIdentifier);