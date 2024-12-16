namespace Poort8.Dataspace.API.Employees.AddNewEmployeeToOrganization;

public record Request(
    string EmployeeId,
    string UseCase,
    string GivenName,
    string FamilyName,
    string Telephone,
    string Email,
    ICollection<EmployeePropertyDto>? Properties);

public record EmployeePropertyDto(
    string Key,
    string Value,
    bool IsIdentifier);

public record Response(
    string EmployeeId,
    string UseCase,
    string GivenName,
    string FamilyName,
    string Telephone,
    string Email,
    ICollection<EmployeePropertyDto>? Properties);
