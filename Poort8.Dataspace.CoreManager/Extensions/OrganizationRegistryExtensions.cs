using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Extensions;

public static class OrganizationRegistryExtensions
{
    public static Organization DeepCopy(this Organization organization)
    {
        return new Organization(
            organization.Identifier,
            organization.Name,
            organization.Adherence.DeepCopy(),
            organization.AdditionalDetails.DeepCopy(),
            organization.AuthorizationRegistries.Select(a => a.DeepCopy()).ToList(),
            organization.Agreements.Select(a => a.DeepCopy()).ToList(),
            organization.Certificates.Select(c => c.DeepCopy()).ToList(),
            organization.Roles.Select(r => r.DeepCopy()).ToList(),
            organization.Properties.Select(p => p.DeepCopy()).ToList());
    }

    private static Adherence DeepCopy(this Adherence adherence)
    {
        return new Adherence(
            adherence.Status,
            adherence.ValidFrom,
            adherence.ValidUntil);
    }

    private static AdditionalDetails DeepCopy(this AdditionalDetails additionalDetails)
    {
        return new AdditionalDetails(
            additionalDetails.Description,
            additionalDetails.WebsiteUrl,
            additionalDetails.CapabilitiesUrl,
            additionalDetails.LogoUrl,
            additionalDetails.CompanyEmail,
            additionalDetails.CompanyPhone,
            additionalDetails.Tags,
            additionalDetails.PubliclyPublishable,
            additionalDetails.CountriesOfOperation.ToList(),
            additionalDetails.Sectors.ToList())
        {
            AdditionalDetailsId = additionalDetails.AdditionalDetailsId
        };
    }

    private static OrganizationRegistry.AuthorizationRegistry DeepCopy(this OrganizationRegistry.AuthorizationRegistry authorizationRegistry)
    {
        return new OrganizationRegistry.AuthorizationRegistry(
            authorizationRegistry.AuthorizationRegistryOrganizationId,
            authorizationRegistry.AuthorizationRegistryUrl,
            authorizationRegistry.DataspaceId)
        {
            AuthorizationRegistryId = authorizationRegistry.AuthorizationRegistryId
        };
    }

    private static Agreement DeepCopy(this Agreement agreement)
    {
        return new Agreement(
            agreement.Type,
            agreement.Title,
            agreement.Status,
            agreement.DateOfSigning,
            agreement.DateOfExpiry,
            agreement.Framework,
            agreement.ContractFile.ToArray(),
            agreement.HashOfSignedContract,
            agreement.CompliancyVerified,
            agreement.DataspaceId)
        {
            AgreementId = agreement.AgreementId
        };
    }

    private static Certificate DeepCopy(this Certificate certificate)
    {
        return new Certificate(
            certificate.CertificateFile.ToArray(),
            certificate.EnabledFrom)
        {
            CertificateId = certificate.CertificateId
        };
    }

    private static OrganizationRole DeepCopy(this OrganizationRole organizationRole)
    {
        return new OrganizationRole(organizationRole.Role, organizationRole.StartDate, organizationRole.EndDate, organizationRole.LoA, organizationRole.LegalAdherence, organizationRole.CompliancyVerified)
        {
            RoleId = organizationRole.RoleId
        };
    }

    private static Property DeepCopy(this Property property)
    {
        return new Property(
            property.Key,
            property.Value,
            property.IsIdentifier)
        {
            PropertyId = property.PropertyId
        };
    }
}