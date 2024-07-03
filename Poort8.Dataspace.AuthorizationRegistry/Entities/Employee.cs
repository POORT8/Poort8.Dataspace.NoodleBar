using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Employee //https://schema.org/Person
{
    [Key]
    public string EmployeeId { get; set; }
    public string UseCase { get; set; } = "default";
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public ICollection<EmployeeProperty> Properties { get; set; } = new List<EmployeeProperty>();

    public Employee(string employeeId, string givenName, string familyName, string telephone, string email)
    {
        EmployeeId = employeeId;
        GivenName = givenName;
        FamilyName = familyName;
        Telephone = telephone;
        Email = email;
    }

    public Employee(string employeeId, string givenName, string familyName, string telephone, string email, ICollection<EmployeeProperty> properties)
        : this(employeeId, givenName, familyName, telephone, email)
    {
        Properties = properties;
    }

    public Employee(string employeeId, string useCase, string givenName, string familyName, string telephone, string email)
        : this(employeeId, givenName, familyName, telephone, email)
    {
        UseCase = useCase;
    }

    public Employee(string employeeId, string useCase, string givenName, string familyName, string telephone, string email, ICollection<EmployeeProperty> properties)
        : this(employeeId, givenName, familyName, telephone, email, properties)
    {
        UseCase = useCase;
    }

    public class EmployeeProperty
    {
        [Key]
        public string PropertyId { get; init; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public EmployeeProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public EmployeeProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }
}