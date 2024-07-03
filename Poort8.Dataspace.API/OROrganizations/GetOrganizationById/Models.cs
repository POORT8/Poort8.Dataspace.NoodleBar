namespace Poort8.Dataspace.API.OROrganizations.GetOrganizationById;

public record AdherenceDto(
    string Status,
    DateOnly ValidFrom,
    DateOnly ValidUntil);

public record AdditionalDetailsDto(
    string? Description,
    string? WebsiteUrl,
    string? CapabilitiesUrl,
    string? LogoUrl,
    string? CompanyEmail,
    string? CompanyPhone,
    string? Tags,
    bool? PubliclyPublishable,
    ICollection<string> CountriesOfOperation,
    ICollection<string> Sectors);

public record AgreementDto(
    string AgreementId,
    string Type,
    string Title,
    string Status,
    DateOnly DateOfSigning,
    DateOnly DateOfExpiry,
    string Framework,
    byte[] ContractFile,
    string HashOfSignedContract,
    bool? CompliancyVerified,
    string? DataspaceId);

public record AuthorizationRegistryDto(
    string AuthorizationRegistryId,
    string AuthorizationRegistryOrganizationId,
    string AuthorizationRegistryUrl,
    string? DataspaceId);

public record CertificateDto(
    string CertificateId,
    byte[] CertificateFile,
    DateOnly EnabledFrom);

public record OrganizationRoleDto(
    string RoleId,
    string Role,
    DateOnly StartDate,
    DateOnly EndDate,
    string LoA,
    bool LegalAdherence,
    bool CompliancyVerified);

public record OrganizationPropertyDto(
    string Key,
    string Value,
    bool IsIdentifier);

public record ServiceDto(
    string ServiceId,
    string Name,
    string? Description,
    string? Url,
    string? LogoUrl,
    string? Color);

public record Response(
    string Identifier,
    string Name,
    AdherenceDto Adherence,
    AdditionalDetailsDto AdditionalDetails,
    ICollection<AgreementDto> Agreements,
    ICollection<AuthorizationRegistryDto> AuthorizationRegistries,
    ICollection<CertificateDto> Certificates,
    ICollection<OrganizationRoleDto> Roles,
    ICollection<OrganizationPropertyDto> Properties,
    ICollection<ServiceDto> Services);
