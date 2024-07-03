using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class ResourceGroup
{
    [Key]
    public string ResourceGroupId { get; set; }
    public string UseCase { get; set; } = "default";
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Provider { get; set; }
    public string? Url { get; set; }
    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    public ICollection<ResourceGroupProperty> Properties { get; set; } = new List<ResourceGroupProperty>();

    public ResourceGroup(string resourceGroupId, string name, string description, string? provider, string? url)
    {
        ResourceGroupId = resourceGroupId;
        Name = name;
        Description = description;
        Provider = provider;
        Url = url;
    }

    public ResourceGroup(string resourceGroupId, string name, string description, string? provider, string? url, ICollection<ResourceGroupProperty> properties)
        : this(resourceGroupId, name, description, provider, url)
    {
        Properties = properties;
    }

    public ResourceGroup(string resourceGroupId, string useCase, string name, string description, string? provider, string? url)
        : this(resourceGroupId, name, description, provider, url)
    {
        UseCase = useCase;
    }

    public ResourceGroup(string resourceGroupId, string useCase, string name, string description, string? provider, string? url, ICollection<Resource> resources)
        : this(resourceGroupId, useCase, name, description, provider, url)
    {
        Resources = resources;
    }

    public ResourceGroup(string resourceGroupId, string useCase, string name, string description, string? provider, string? url, ICollection<Resource> resources, ICollection<ResourceGroupProperty> properties)
        : this(resourceGroupId, useCase, name, description, provider, url, resources)
    {
        Properties = properties;
    }

    public class ResourceGroupProperty
    {
        [Key]
        public string PropertyId { get; init; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public ResourceGroupProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public ResourceGroupProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }
}