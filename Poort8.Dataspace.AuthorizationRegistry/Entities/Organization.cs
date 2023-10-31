using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Organization //https://schema.org/Organization
{
    [Key]
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Representative { get; set; }
    public string InvoicingContact { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Organization(string identifier, string name, string url, string representative, string invoicingContact)
    {
        Identifier = identifier;
        Name = name;
        Url = url;
        Representative = representative;
        InvoicingContact = invoicingContact;
    }

    public Organization(string identifier, string name, string url, string representative, string invoicingContact, ICollection<Property> properties)
    {
        Identifier = identifier;
        Name = name;
        Url = url;
        Representative = representative;
        InvoicingContact = invoicingContact;
        Properties = properties;
    }
}