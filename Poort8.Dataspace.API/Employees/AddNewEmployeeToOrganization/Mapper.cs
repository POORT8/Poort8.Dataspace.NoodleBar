using FastEndpoints;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.API.Employees.AddNewEmployeeToOrganization;

public class Mapper : Mapper<Request, Response, Employee>
{
    public override Employee ToEntity(Request request)
    {
        return new Employee(
            request.EmployeeId,
            request.UseCase,
            request.GivenName,
            request.FamilyName,
            request.Telephone,
            request.Email,
            request.Properties?.Select(p => new Employee.EmployeeProperty(p.Key, p.Value, p.IsIdentifier)).ToList() ?? []);
    }

    public override Response FromEntity(Employee employee)
    {
        return new Response(
            employee.EmployeeId,
            employee.UseCase,
            employee.GivenName,
            employee.FamilyName,
            employee.Telephone,
            employee.Email,
            employee.Properties.Select(
                p => new EmployeePropertyDto(p.Key, p.Value, p.IsIdentifier)).ToList());
    }
}
