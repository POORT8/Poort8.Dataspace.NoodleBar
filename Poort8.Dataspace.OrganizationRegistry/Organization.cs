using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.OrganizationRegistry;
public class Organization //https://schema.org/Organization
{
    [Key]
    public string Identifier { get; set; }
    public string Name { get; set; }
    public Adherence Adherence { get; set; } = new Adherence("active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddYears(1)));
    public ICollection<OrganizationRole> Roles { get; set; } = new List<OrganizationRole>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Organization(string identifier, string name)
    {
        Identifier = identifier;
        Name = name;
    }

    public Organization(string identifier, string name, Adherence adherence, ICollection<OrganizationRole> roles, ICollection<Property> properties)
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
    public string Status { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidUntil { get; set; }

    public Adherence(string status, DateOnly validFrom, DateOnly validUntil)
    {
        Status = status;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }
}

public class OrganizationRole
{
    [Key]
    public string RoleId { get; set; } = Guid.NewGuid().ToString();
    public string Role { get; set; }

    public OrganizationRole(string role)
    {
        Role = role;
    }
}

public class Property
{
    [Key]
    public string PropertyId { get; init; } = Guid.NewGuid().ToString();
    public string Key { get; set; }
    public string Value { get; set; }
    public bool IsIdentifier { get; set; } = false;

    public Property(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public Property(string key, string value, bool isIdentifier)
    {
        Key = key;
        Value = value;
        IsIdentifier = isIdentifier;
    }
}