using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Feature //https://schema.org/Service
{
    [Key]
    public string FeatureId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Feature()
    {
    }

    public Feature(string featureId, string name, string description)
    {
        FeatureId = featureId;
        Name = name;
        Description = description;
    }

    public Feature(string featureId, string name, string description, ICollection<Property> properties)
    {
        FeatureId = featureId;
        Name = name;
        Description = description;
        Properties = properties;
    }

    public Feature DeepCopy()
    {
        return new Feature(
            FeatureId,
            Name,
            Description,
            Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            Products = Products.Select(p => p).ToList()
        };
    }
}