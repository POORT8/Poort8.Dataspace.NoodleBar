using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.OrganizationRegistry;
public class Organization //https://schema.org/Organization
{
    [Key]
    public string Identifier { get; set; }
    public string Name { get; set; }
    public Adherence Adherence { get; set; } = new Adherence("Active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddYears(1)));
    public AdditionalDetails AdditionalDetails { get; set; } = new AdditionalDetails();
    public ICollection<AuthorizationRegistry> AuthorizationRegistries { get; set; } = new List<AuthorizationRegistry>();
    public ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    public ICollection<OrganizationRole> Roles { get; set; } = new List<OrganizationRole>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Organization(string identifier, string name)
    {
        Identifier = identifier;
        Name = name;
    }

    public Organization(string identifier, string name, Adherence adherence, ICollection<OrganizationRole> roles, ICollection<Property> properties)
    {
        Identifier = identifier;
        Name = name;
        Adherence = adherence;
        Roles = roles;
        Properties = properties;
    }

    public Organization(
        string identifier,
        string name,
        Adherence adherence,
        AdditionalDetails additionalDetails,
        ICollection<AuthorizationRegistry> authorizationRegistries,
        ICollection<Agreement> agreements,
        ICollection<Certificate> certificates,
        ICollection<OrganizationRole> roles,
        ICollection<Property> properties)
    {
        Identifier = identifier;
        Name = name;
        Adherence = adherence;
        AdditionalDetails = additionalDetails;
        AuthorizationRegistries = authorizationRegistries;
        Agreements = agreements;
        Certificates = certificates;
        Roles = roles;
        Properties = properties;
    }
}

public class Adherence
{
    public string Status { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidUntil { get; set; }

    public Adherence(string status, DateOnly validFrom, DateOnly validUntil)
    {
        Status = status;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }
}

public class AdditionalDetails
{
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? CapabilitiesUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyPhone { get; set; }
    public string? Tags { get; set; }
    public bool? PubliclyPublishable { get; set; }
    public ICollection<string> CountriesOfOperation { get; set; } = new List<string>();
    public ICollection<string> Sectors { get; set; } = new List<string>();

    public AdditionalDetails(
        string? description = null,
        string? websiteUrl = null,
        string? capabilitiesUrl = null,
        string? logoUrl = null,
        string? companyEmail = null,
        string? companyPhone = null,
        string? tags = null,
        bool? publiclyPublishable = null,
        ICollection<string>? countriesOfOperation = null,
        ICollection<string>? sectors = null)
    {
        Description = description;
        WebsiteUrl = websiteUrl;
        CapabilitiesUrl = capabilitiesUrl;
        LogoUrl = logoUrl;
        CompanyEmail = companyEmail;
        CompanyPhone = companyPhone;
        Tags = tags;
        PubliclyPublishable = publiclyPublishable;
        if (countriesOfOperation is not null) CountriesOfOperation = countriesOfOperation;
        if (sectors is not null) Sectors = sectors;
    }
}

public class AuthorizationRegistry
{
    [Key]
    public string AuthorizationRegistryId { get; set; } = Guid.NewGuid().ToString();
    public string AuthorizationRegistryOrganizationId { get; set; }
    public string AuthorizationRegistryUrl { get; set; }
    public string? DataspaceId { get; set; }

    public AuthorizationRegistry(string authorizationRegistryOrganizationId, string authorizationRegistryUrl, string? dataspaceId = null)
    {
        AuthorizationRegistryOrganizationId = authorizationRegistryOrganizationId;
        AuthorizationRegistryUrl = authorizationRegistryUrl;
        DataspaceId = dataspaceId;
    }
}

public class Agreement
{
    [Key]
    public string AgreementId { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public DateOnly DateOfSigning { get; set; }
    public DateOnly DateOfExpiry { get; set; }
    public string Framework { get; set; }
    public byte[] ContractFile { get; set; }
    public string HashOfSignedContract { get; set; }
    public bool? CompliancyVerified { get; set; }
    public string? DataspaceId { get; set; }

    public Agreement(
        string type,
        string title,
        string status,
        DateOnly dateOfSigning,
        DateOnly dateOfExpiry,
        string framework,
        byte[] contractFile,
        string hashOfSignedContract,
        bool? compliancyVerified,
        string? dataspaceId = null)
    {
        Type = type;
        Title = title;
        Status = status;
        DateOfSigning = dateOfSigning;
        DateOfExpiry = dateOfExpiry;
        Framework = framework;
        ContractFile = contractFile;
        HashOfSignedContract = hashOfSignedContract;
        CompliancyVerified = compliancyVerified;
        DataspaceId = dataspaceId;
    }
}

public class Certificate
{
    [Key]
    public string CertificateId { get; set; } = Guid.NewGuid().ToString();
    public byte[] CertificateFile { get; set; }
    public DateOnly EnabledFrom { get; set; }

    public Certificate(byte[] certificateFile, DateOnly enabledFrom)
    {
        CertificateFile = certificateFile;
        EnabledFrom = enabledFrom;
    }
}

public class OrganizationRole
{
    [Key]
    public string RoleId { get; set; } = Guid.NewGuid().ToString();
    public string Role { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.MinValue;
    public DateOnly EndDate { get; set; } = DateOnly.MaxValue;
    public string LoA { get; set; } = string.Empty;
    public bool LegalAdherence { get; set; } = false;
    public bool CompliancyVerified { get; set; } = false;

    public OrganizationRole(string role)
    {
        Role = role;
    }

    public OrganizationRole(
        string role,
        DateOnly startDate,
        DateOnly endDate,
        string loA,
        bool legalAdherence,
        bool compliancyVerified)
    {
        Role = role;
        StartDate = startDate;
        EndDate = endDate;
        LoA = loA;
        LegalAdherence = legalAdherence;
        CompliancyVerified = compliancyVerified;
    }
}

public class Property
{
    [Key]
    public string PropertyId { get; init; } = Guid.NewGuid().ToString();
    public string Key { get; set; }
    public string Value { get; set; }
    public bool IsIdentifier { get; set; } = false;

    public Property(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public Property(string key, string value, bool isIdentifier)
    {
        Key = key;
        Value = value;
        IsIdentifier = isIdentifier;
    }
}