using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Service //https://schema.org/Service
{
    [Key]
    public string ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProductId { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Service()
    {
    }

    public Service(string serviceId, string name, string description, string productId, ICollection<Product> products)
    {
        ServiceId = serviceId;
        Name = name;
        Description = description;
        ProductId = productId;
        Products = products;
    }

    public Service(string serviceId, string name, string description, string productId, ICollection<Product> products, ICollection<Property> properties)
    {
        ServiceId = serviceId;
        Name = name;
        Description = description;
        ProductId = productId;
        Products = products;
        Properties = properties;
    }
}