using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Resource
{
    [Key]
    public string ResourceId { get; set; }
    public string UseCase { get; set; } = "default";
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<ResourceProperty> Properties { get; set; } = new List<ResourceProperty>();

    public Resource(string resourceId, string name, string description)
    {
        ResourceId = resourceId;
        Name = name;
        Description = description;
    }

    public Resource(string resourceId, string name, string description, ICollection<ResourceProperty> properties)
        : this(resourceId, name, description)
    {
        Properties = properties;
    }

    public Resource(string resourceId, string useCase, string name, string description)
        : this(resourceId, name, description)
    {
        UseCase = useCase;
    }

    public Resource(string resourceId, string useCase, string name, string description, ICollection<ResourceProperty> properties)
        : this(resourceId, useCase, name, description)
    {
        Properties = properties;
    }

    public class ResourceProperty
    {
        [Key]
        public string PropertyId { get; init; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public ResourceProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public ResourceProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }
}