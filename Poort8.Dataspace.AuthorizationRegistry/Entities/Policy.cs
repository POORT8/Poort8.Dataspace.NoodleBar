using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Policy //TODO: or Subscription?
{
    [Key]
    public string PolicyId { get; init; } = Guid.NewGuid().ToString();
    public DateTime IssuedAt { get; set; } = DateTime.Now;
    public DateTime NotBefore { get; set; } = DateTime.Now;
    public DateTime Expiration { get; set; } = DateTime.Now.AddYears(1);
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Policy(string issuer, string subject, string resource, string action)
    {
        Issuer = issuer;
        Subject = subject;
        Resource = resource;
        Action = action;
    }

    public Policy(string issuer, string subject, string resource, string action, ICollection<Property> properties)
    {
        Issuer = issuer;
        Subject = subject;
        Resource = resource;
        Action = action;
        Properties = properties;
    }
}