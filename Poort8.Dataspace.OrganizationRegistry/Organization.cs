using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.OrganizationRegistry;
public class Organization //https://schema.org/Organization
{
    [Key]
    public string Identifier { get; set; }
    public string Name { get; set; }
    public Adherence Adherence { get; set; } = new();
    public ICollection<OrganizationRole> Roles { get; set; } = new List<OrganizationRole>();
    public ICollection<OrganizationProperty> Properties { get; set; } = new List<OrganizationProperty>();

    public Organization(string identifier, string name)
    {
        Identifier = identifier;
        Name = name;
    }

    public Organization(string identifier, string name, Adherence adherence, ICollection<OrganizationRole> roles, ICollection<OrganizationProperty> properties)
    {
        Identifier = identifier;
        Name = name;
        Adherence = adherence;
        Roles = roles;
        Properties = properties;
    }
}

public class Adherence
{
    [Key]
    public string AdherenceId { get; set; } = Guid.NewGuid().ToString();
    public string Status { get; set; } = "active";
    public DateOnly ValidFrom { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public DateOnly ValidUntil { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
}

public class OrganizationRole
{
    [Key]
    public string RoleId { get; set; } = Guid.NewGuid().ToString();
    public string Role { get; set; } = "member";
}

public class OrganizationProperty
{
    [Key]
    public string PropertyId { get; set; } = Guid.NewGuid().ToString();
    public string Key { get; set; }
    public string Value { get; set; }

    public OrganizationProperty(string key, string value)
    {
        Key = key;
        Value = value;
    }
}