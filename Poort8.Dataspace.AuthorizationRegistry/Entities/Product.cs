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
    public ICollection<ProductProperty> Properties { get; set; } = new List<ProductProperty>();

    public Product(string productId, string name, string description, string? provider, string? url)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Provider = provider;
        Url = url;
    }

    public Product(string productId, string name, string description, string? provider, string? url, ICollection<ProductProperty> properties)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Provider = provider;
        Url = url;
        Properties = properties;
    }

    public class ProductProperty
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public ProductProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public ProductProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }
}