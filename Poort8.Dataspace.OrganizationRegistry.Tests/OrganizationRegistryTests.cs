using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.OrganizationRegistry.Extensions;

namespace Poort8.Dataspace.OrganizationRegistry.Tests;

public class OrganizationRegistryTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IOrganizationRegistry _organizationRegistry;

    public OrganizationRegistryTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOrganizationRegistrySqlite(options => options.ConnectionString = $"Data Source={Guid.NewGuid()}.db");
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _organizationRegistry = _serviceProvider.GetRequiredService<IOrganizationRegistry>();

        var factory = _serviceProvider.GetRequiredService<IDbContextFactory<OrganizationContext>>();
        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }

    public static Organization CreateNewOrganization(string id, int index)
    {
        var adherence = new Adherence("Active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddYears(1)));
        var roles = new List<OrganizationRole>() { new OrganizationRole("role") };
        var properties = new List<Property>() { new Property("key", "value"), new Property("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Organization($"{id}-id", $"{id}{index}-name", adherence, roles, properties);
    }

    private static Agreement CreateNewAgreement()
    {
        return new Agreement("Type", "Title", "Status", DateOnly.MinValue, DateOnly.MaxValue, "Framework", Array.Empty<byte>(), "hashOfContract", null);
    }

    private static AuthorizationRegistry CreateNewAuthorizationRegistry()
    {
        return new AuthorizationRegistry("OrganizationID", "url");
    }

    private static Certificate CreateNewCertificate()
    {
        return new Certificate(System.Text.Encoding.UTF8.GetBytes("certificate"), DateOnly.FromDateTime(DateTime.MinValue));
    }

    private static OrganizationRole CreateNewRole()
    {
        return new OrganizationRole("role");
    }

    private static Property CreateNewProperty()
    {
        return new Property("key", "value");
    }

    private static Service CreateNewService()
    {
        return new Service("service");
    }

    [Fact]
    public async Task CreateOrganization()
    {
        var organization = CreateNewOrganization(nameof(CreateOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var readEntity = await _organizationRegistry.ReadOrganization(organizationEntity.Identifier);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteOrganization(organizationEntity.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task CreateAndReadOrganization()
    {
        var organization = CreateNewOrganization(nameof(CreateOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);

        var readEntity = await _organizationRegistry.ReadOrganization(organization.Identifier);

        Assert.NotNull(readEntity);
        Assert.Equal(organization.Identifier, readEntity.Identifier);

        var readByPropIdEntity = await _organizationRegistry.ReadOrganization(organization.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(organization.Identifier, readByPropIdEntity.Identifier);

        var readByPropEntity = await _organizationRegistry.ReadOrganizations(propertyKey: "key", propertyValue: "value");

        Assert.NotNull(readByPropEntity);
        Assert.Single(readByPropEntity);
        Assert.Equal(organization.Identifier, readByPropEntity[0].Identifier);

        var readByNameEntity = await _organizationRegistry.ReadOrganizations(name: organization.Name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(organization.Identifier, readByNameEntity[0].Identifier);

        var newName = Guid.NewGuid().ToString();
        var organizationUpdate = new Organization(organization.Identifier, newName);
        var updateEntity = await _organizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);

        readEntity = await _organizationRegistry.ReadOrganization(organization.Identifier);

        Assert.NotNull(readEntity);
        Assert.Equal(organization.Identifier, readEntity.Identifier);
        Assert.Equal(newName, readEntity.Name);

        var success = await _organizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task CreateAndUpdateOrganization()
    {
        var organization = CreateNewOrganization(nameof(CreateOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.AuthorizationRegistries);
        Assert.Equal(organization.AuthorizationRegistries.Count, organizationEntity.AuthorizationRegistries.Count);
        Assert.NotNull(organizationEntity.Agreements);
        Assert.Equal(organization.Agreements.Count, organizationEntity.Agreements.Count);
        Assert.NotNull(organizationEntity.Certificates);
        Assert.Equal(organization.Certificates.Count, organizationEntity.Certificates.Count);
        Assert.NotNull(organizationEntity.Roles);
        Assert.Equal(organization.Roles.Count, organizationEntity.Roles.Count);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);
        Assert.NotNull(organizationEntity.Services);
        Assert.Equal(organization.Services.Count, organizationEntity.Services.Count);

        var newName = "NewName";
        var newStatus = "Not Active";
        var newEmail = "hello@abctrucking.nl";
        var newCountries = new List<string> { "Netherlands", "Belgium" };

        organization.Name = newName;
        organization.Adherence.Status = newStatus;
        organization.AdditionalDetails.CompanyEmail = newEmail;
        organization.AdditionalDetails.CountriesOfOperation = newCountries;
        var updateEntity = await _organizationRegistry.UpdateOrganization(organization);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Equal(newName, updateEntity.Name);
        Assert.Equal(newStatus, updateEntity.Adherence.Status);
        Assert.Equal(newEmail, updateEntity.AdditionalDetails.CompanyEmail);
        Assert.Equal(newCountries, updateEntity.AdditionalDetails.CountriesOfOperation);

        var success = await _organizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAgreementToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAgreementToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var agreement = CreateNewAgreement();
        var agreementEntity = await _organizationRegistry.AddNewAgreementToOrganization(organizationEntity.Identifier, agreement);
        Assert.NotNull(agreementEntity);

        var readEntity = await _organizationRegistry.ReadAgreement(agreementEntity.AgreementId);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteAgreement(agreementEntity.AgreementId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAndUpdateAgreementToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAndUpdateAgreementToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var agreement = CreateNewAgreement();
        var agreementEntity = await _organizationRegistry.AddNewAgreementToOrganization(organizationEntity.Identifier, agreement);
        Assert.NotNull(agreementEntity);

        var newTitle = "New Title";
        var newDateOfSigning = DateOnly.FromDateTime(DateTime.Now);
        var newContract = System.Text.Encoding.UTF8.GetBytes("NewContract");

        agreementEntity.Title = newTitle;
        agreementEntity.DateOfSigning = newDateOfSigning;
        agreementEntity.ContractFile = newContract;
        agreement.CompliancyVerified = true;
        var updateEntity = await _organizationRegistry.UpdateAgreement(agreementEntity);
        Assert.NotNull(updateEntity);
        Assert.Equal(agreementEntity.AgreementId, updateEntity.AgreementId);
        Assert.Equal(newTitle, updateEntity.Title);
        Assert.Equal(newDateOfSigning, updateEntity.DateOfSigning);
        Assert.Equal(newContract, updateEntity.ContractFile);
        Assert.True(updateEntity.CompliancyVerified);

        var readEntity = await _organizationRegistry.ReadAgreement(agreementEntity.AgreementId);
        Assert.NotNull(readEntity);
        Assert.Equal(agreementEntity.AgreementId, readEntity.AgreementId);
        Assert.Equal(newTitle, readEntity.Title);
        Assert.Equal(newDateOfSigning, readEntity.DateOfSigning);
        Assert.Equal(newContract, readEntity.ContractFile);
        Assert.True(readEntity.CompliancyVerified);

        var success = await _organizationRegistry.DeleteAgreement(agreementEntity.AgreementId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAuthorizationRegistryToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAuthorizationRegistryToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var authorizationRegistry = CreateNewAuthorizationRegistry();
        var authorizationRegistryEntity = await _organizationRegistry.AddNewAuthorizationRegistryToOrganization(organizationEntity.Identifier, authorizationRegistry);
        Assert.NotNull(authorizationRegistryEntity);

        var readEntity = await _organizationRegistry.ReadAuthorizationRegistry(authorizationRegistryEntity.AuthorizationRegistryId);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteAuthorizationRegistry(authorizationRegistryEntity.AuthorizationRegistryId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAndUpdateAuthorizationRegistryToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAndUpdateAuthorizationRegistryToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var authorizationRegistry = CreateNewAuthorizationRegistry();
        var authorizationRegistryEntity = await _organizationRegistry.AddNewAuthorizationRegistryToOrganization(organizationEntity.Identifier, authorizationRegistry);
        Assert.NotNull(authorizationRegistryEntity);

        var newUrl = "new.url";
        var newDataspaceId = "theDataspace";

        authorizationRegistryEntity.AuthorizationRegistryUrl = newUrl;
        authorizationRegistryEntity.DataspaceId = newDataspaceId;
        var updateEntity = await _organizationRegistry.UpdateAuthorizationRegistry(authorizationRegistryEntity);
        Assert.NotNull(updateEntity);
        Assert.Equal(authorizationRegistryEntity.AuthorizationRegistryId, updateEntity.AuthorizationRegistryId);
        Assert.Equal(newUrl, updateEntity.AuthorizationRegistryUrl);
        Assert.Equal(newDataspaceId, updateEntity.DataspaceId);

        var readEntity = await _organizationRegistry.ReadAuthorizationRegistry(authorizationRegistryEntity.AuthorizationRegistryId);
        Assert.NotNull(readEntity);
        Assert.Equal(authorizationRegistryEntity.AuthorizationRegistryId, readEntity.AuthorizationRegistryId);
        Assert.Equal(newUrl, readEntity.AuthorizationRegistryUrl);
        Assert.Equal(newDataspaceId, readEntity.DataspaceId);

        var success = await _organizationRegistry.DeleteAuthorizationRegistry(authorizationRegistryEntity.AuthorizationRegistryId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddCertificateToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddCertificateToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var certificate = CreateNewCertificate();
        var certificateEntity = await _organizationRegistry.AddNewCertificateToOrganization(organizationEntity.Identifier, certificate);
        Assert.NotNull(certificateEntity);

        var readEntity = await _organizationRegistry.ReadCertificate(certificateEntity.CertificateId);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteCertificate(certificateEntity.CertificateId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAndUpdateCertificateToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAndUpdateCertificateToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var certificate = CreateNewCertificate();
        var certificateEntity = await _organizationRegistry.AddNewCertificateToOrganization(organizationEntity.Identifier, certificate);
        Assert.NotNull(certificateEntity);

        var newCertificate = System.Text.Encoding.UTF8.GetBytes("newCertificate");
        var newEnabledFrom = DateOnly.FromDateTime(DateTime.Now);

        certificateEntity.CertificateFile = newCertificate;
        certificateEntity.EnabledFrom = newEnabledFrom;
        var updateEntity = await _organizationRegistry.UpdateCertificate(certificateEntity);
        Assert.NotNull(updateEntity);
        Assert.Equal(certificateEntity.CertificateId, updateEntity.CertificateId);
        Assert.Equal(newCertificate, updateEntity.CertificateFile);
        Assert.Equal(newEnabledFrom, updateEntity.EnabledFrom);

        var readEntity = await _organizationRegistry.ReadCertificate(certificateEntity.CertificateId);
        Assert.NotNull(readEntity);
        Assert.Equal(certificateEntity.CertificateId, readEntity.CertificateId);
        Assert.Equal(newCertificate, readEntity.CertificateFile);
        Assert.Equal(newEnabledFrom, readEntity.EnabledFrom);

        var success = await _organizationRegistry.DeleteCertificate(certificateEntity.CertificateId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddRoleToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddRoleToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var role = CreateNewRole();
        var roleEntity = await _organizationRegistry.AddNewRoleToOrganization(organizationEntity.Identifier, role);
        Assert.NotNull(roleEntity);

        var readEntity = await _organizationRegistry.ReadRole(roleEntity.RoleId);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteRole(roleEntity.RoleId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAndUpdateRoleToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAndUpdateRoleToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var role = CreateNewRole();
        var roleEntity = await _organizationRegistry.AddNewRoleToOrganization(organizationEntity.Identifier, role);
        Assert.NotNull(roleEntity);

        var newRole = "newRole";
        var newStartDate = DateOnly.FromDateTime(DateTime.Now);
        var newLoA = "newLoA";

        roleEntity.Role = newRole;
        roleEntity.StartDate = newStartDate;
        roleEntity.LoA = newLoA;
        roleEntity.CompliancyVerified = true;
        var updateEntity = await _organizationRegistry.UpdateRole(roleEntity);
        Assert.NotNull(updateEntity);
        Assert.Equal(roleEntity.RoleId, updateEntity.RoleId);
        Assert.Equal(newRole, updateEntity.Role);
        Assert.Equal(newStartDate, updateEntity.StartDate);
        Assert.Equal(newLoA, updateEntity.LoA);
        Assert.True(updateEntity.CompliancyVerified);

        var readEntity = await _organizationRegistry.ReadRole(roleEntity.RoleId);
        Assert.NotNull(readEntity);
        Assert.Equal(roleEntity.RoleId, readEntity.RoleId);
        Assert.Equal(newRole, readEntity.Role);
        Assert.Equal(newStartDate, readEntity.StartDate);
        Assert.Equal(newLoA, readEntity.LoA);
        Assert.True(readEntity.CompliancyVerified);

        var success = await _organizationRegistry.DeleteRole(roleEntity.RoleId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddPropertyToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddPropertyToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var property = CreateNewProperty();
        var propertyEntity = await _organizationRegistry.AddNewPropertyToOrganization(organizationEntity.Identifier, property);
        Assert.NotNull(propertyEntity);

        var readEntity = await _organizationRegistry.ReadProperty(propertyEntity.PropertyId);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteProperty(propertyEntity.PropertyId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAndUpdatePropertyToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAndUpdatePropertyToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var property = CreateNewProperty();
        var propertyEntity = await _organizationRegistry.AddNewPropertyToOrganization(organizationEntity.Identifier, property);
        Assert.NotNull(propertyEntity);

        var newValue = "newValue";

        propertyEntity.Value = newValue;
        propertyEntity.IsIdentifier = true;
        var updateEntity = await _organizationRegistry.UpdateProperty(propertyEntity);
        Assert.NotNull(updateEntity);
        Assert.Equal(propertyEntity.PropertyId, updateEntity.PropertyId);
        Assert.Equal(newValue, updateEntity.Value);
        Assert.True(updateEntity.IsIdentifier);

        var readEntity = await _organizationRegistry.ReadProperty(propertyEntity.PropertyId);
        Assert.NotNull(readEntity);
        Assert.Equal(propertyEntity.PropertyId, readEntity.PropertyId);
        Assert.Equal(newValue, readEntity.Value);
        Assert.True(readEntity.IsIdentifier);

        var success = await _organizationRegistry.DeleteProperty(propertyEntity.PropertyId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddServiceToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddServiceToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var service = CreateNewService();
        var serviceEntity = await _organizationRegistry.AddNewServiceToOrganization(organizationEntity.Identifier, service);
        Assert.NotNull(serviceEntity);

        var readEntity = await _organizationRegistry.ReadService(serviceEntity.ServiceId);
        Assert.NotNull(readEntity);

        var success = await _organizationRegistry.DeleteService(serviceEntity.ServiceId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddAndUpdateServiceToOrganization()
    {
        var organization = CreateNewOrganization(nameof(AddAndUpdateServiceToOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        var service = CreateNewService();
        var serviceEntity = await _organizationRegistry.AddNewServiceToOrganization(organizationEntity.Identifier, service);
        Assert.NotNull(serviceEntity);

        var newName = "newName";
        var newColor = "Blue";

        serviceEntity.Name = newName;
        serviceEntity.Color = newColor;
        var updateEntity = await _organizationRegistry.UpdateService(serviceEntity);
        Assert.NotNull(updateEntity);
        Assert.Equal(serviceEntity.ServiceId, updateEntity.ServiceId);
        Assert.Equal(newName, updateEntity.Name);
        Assert.Equal(newColor, updateEntity.Color);

        var readEntity = await _organizationRegistry.ReadService(serviceEntity.ServiceId);
        Assert.NotNull(readEntity);
        Assert.Equal(serviceEntity.ServiceId, readEntity.ServiceId);
        Assert.Equal(newName, readEntity.Name);
        Assert.Equal(newColor, readEntity.Color);

        var success = await _organizationRegistry.DeleteService(serviceEntity.ServiceId);
        Assert.True(success);
    }

    [Fact]
    public async Task DeleteRoleAndPropertiesOrganization()
    {
        var organization = CreateNewOrganization(nameof(DeleteRoleAndPropertiesOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);

        foreach (var role in organizationEntity.Roles)
        {
            await _organizationRegistry.DeleteRole(role.RoleId);
        }

        foreach (var property in organizationEntity.Properties)
        {
            await _organizationRegistry.DeleteProperty(property.PropertyId);
        }

        var updatedEntity = await _organizationRegistry.ReadOrganization(organization.Identifier);

        Assert.NotNull(updatedEntity);
        Assert.Equal(organization.Identifier, updatedEntity.Identifier);
        Assert.Empty(updatedEntity.Roles);
        Assert.Empty(updatedEntity.Properties);

        var success = await _organizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }
}