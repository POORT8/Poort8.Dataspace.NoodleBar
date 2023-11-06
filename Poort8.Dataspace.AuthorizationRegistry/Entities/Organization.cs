using Microsoft.EntityFrameworkCore;
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
    public ICollection<OrganizationProperty> Properties { get; set; } = new List<OrganizationProperty>();

    public Organization(string identifier, string name, string url, string representative, string invoicingContact)
    {
        Identifier = identifier;
        Name = name;
        Url = url;
        Representative = representative;
        InvoicingContact = invoicingContact;
    }

    public Organization(string identifier, string name, string url, string representative, string invoicingContact, ICollection<OrganizationProperty> properties)
    {
        Identifier = identifier;
        Name = name;
        Url = url;
        Representative = representative;
        InvoicingContact = invoicingContact;
        Properties = properties;
    }

    [Owned]
    public class OrganizationProperty
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public OrganizationProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public OrganizationProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }
}