using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.OrganizationRegistry;
public class Organization
{
    [Key]
    public string Identifier { get; set; }
    public string Name { get; set; }
    public List<OrganizationProperty> Properties { get; set; }

    public Organization(string identifier, string name)
    {
        Identifier = identifier;
        Name = name;
        Properties = new List<OrganizationProperty>();
    }

    public Organization(string identifier, string name, List<OrganizationProperty> properties)
    {
        Identifier = identifier;
        Name = name;
        Properties = properties;
    }
}

public class OrganizationProperty
{
    [Key]
    public string PropertyId { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    public OrganizationProperty(string propertyId, string key, string value)
    {
        PropertyId = propertyId;
        Key = key;
        Value = value;
    }
}