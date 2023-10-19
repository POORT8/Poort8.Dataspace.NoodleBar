using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Product //https://schema.org/Product
{
    [Key]
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Provider { get; set; }
    public string? Url { get; set; }
    public ICollection<Feature> Features { get; set; } = new List<Feature>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Product(string productId, string name, string description, string? provider, string? url)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Provider = provider;
        Url = url;
    }

    public Product(string productId, string name, string description, string? provider, string? url, ICollection<Property> properties)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Provider = provider;
        Url = url;
        Properties = properties;
    }

    public Product DeepCopy()
    {
        return new(
            ProductId,
            Name,
            Description,
            Provider,
            Url,
            Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            Features = Features.Select(f => f).ToList()
        };
    }
}