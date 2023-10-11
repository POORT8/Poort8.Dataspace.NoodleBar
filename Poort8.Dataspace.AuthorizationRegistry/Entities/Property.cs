using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
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