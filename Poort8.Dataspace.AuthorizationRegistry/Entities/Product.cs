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
    public ICollection<Action> PotentialActions { get; set; } = new List<Action>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
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
}