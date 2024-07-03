using FastEndpoints;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.API.OROrganizations.GetOrganizationById;

public class Mapper : ResponseMapper<Response, Organization>
{
    public override Response FromEntity(Organization organization)
    {
        return new Response(
            organization.Identifier,
            organization.Name,
            new AdherenceDto(
                organization.Adherence.Status,
                organization.Adherence.ValidFrom,
                organization.Adherence.ValidUntil
            ),
            new AdditionalDetailsDto(
                organization.AdditionalDetails.Description,
                organization.AdditionalDetails.WebsiteUrl,
                organization.AdditionalDetails.CapabilitiesUrl,
                organization.AdditionalDetails.LogoUrl,
                organization.AdditionalDetails.CompanyEmail,
                organization.AdditionalDetails.CompanyPhone,
                organization.AdditionalDetails.Tags,
                organization.AdditionalDetails.PubliclyPublishable,
                organization.AdditionalDetails.CountriesOfOperation.ToList(),
                organization.AdditionalDetails.Sectors.ToList()
            ),
            organization.Agreements.Select(a => new AgreementDto(
                a.AgreementId,
                a.Type,
                a.Title,
                a.Status,
                a.DateOfSigning,
                a.DateOfExpiry,
                a.Framework,
                a.ContractFile,
                a.HashOfSignedContract,
                a.CompliancyVerified,
                a.DataspaceId
            )).ToList(),
            organization.AuthorizationRegistries.Select(ar => new AuthorizationRegistryDto(
                ar.AuthorizationRegistryId,
                ar.AuthorizationRegistryOrganizationId,
                ar.AuthorizationRegistryUrl,
                ar.DataspaceId
            )).ToList(),
            organization.Certificates.Select(c => new CertificateDto(
                c.CertificateId,
                c.CertificateFile,
                c.EnabledFrom
            )).ToList(),
            organization.Roles.Select(r => new OrganizationRoleDto(
                r.RoleId,
                r.Role,
                r.StartDate,
                r.EndDate,
                r.LoA,
                r.LegalAdherence,
                r.CompliancyVerified
            )).ToList(),
            organization.Properties.Select(p => new OrganizationPropertyDto(
                p.Key,
                p.Value,
                p.IsIdentifier
            )).ToList(),
            organization.Services.Select(s => new ServiceDto(
                s.ServiceId,
                s.Name,
                s.Description,
                s.Url,
                s.LogoUrl,
                s.Color
            )).ToList()
        );
    }
}
