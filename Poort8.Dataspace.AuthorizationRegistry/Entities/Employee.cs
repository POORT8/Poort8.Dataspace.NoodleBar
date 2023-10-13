using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Employee //https://schema.org/Person
{
    [Key]
    public string EmployeeId { get; set; }
    public string OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public Organization WorksFor => Organization;
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Employee()
    {
    }

    public Employee(string employeeId, string givenName, string familyName, string telephone, string email)
    {
        EmployeeId = employeeId;
        GivenName = givenName;
        FamilyName = familyName;
        Telephone = telephone;
        Email = email;
    }

    public Employee(string employeeId, string givenName, string familyName, string telephone, string email, ICollection<Property> properties)
    {
        EmployeeId = employeeId;
        GivenName = givenName;
        FamilyName = familyName;
        Telephone = telephone;
        Email = email;
        Properties = properties;
    }
}