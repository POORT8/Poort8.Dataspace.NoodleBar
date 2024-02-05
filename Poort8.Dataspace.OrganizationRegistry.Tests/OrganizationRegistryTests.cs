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

        organization.Adherence.Status = "Not Active";
        organization.AdditionalDetails.CompanyEmail = "hello@abctrucking.nl";
        organization.AuthorizationRegistries.Add(new AuthorizationRegistry("arId", "www.ar.com"));
        organization.Agreements.Add(new Agreement("Terms Of Use", "title", "Signed", DateOnly.MinValue, DateOnly.MaxValue, "framework", Array.Empty<byte>(), "hash", false));
        organization.Certificates.Add(new Certificate(Array.Empty<byte>(), DateOnly.MinValue));
        organization.Roles.Add(new OrganizationRole("otherRole"));
        organization.Properties.Add(new Property("otherKey", "otherValue"));
        var updateEntity = await _organizationRegistry.UpdateOrganization(organization);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Equal("Not Active", updateEntity.Adherence.Status);
        Assert.Equal("hello@abctrucking.nl", updateEntity.AdditionalDetails.CompanyEmail);
        Assert.Single(updateEntity.AuthorizationRegistries);
        Assert.Single(updateEntity.Agreements);
        Assert.Single(updateEntity.Certificates);
        Assert.Equal(2, updateEntity.Roles.Count);
        Assert.Equal(3, updateEntity.Properties.Count);

        var success = await _organizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task DeleteRoleAndPropertiesOrganization()
    {
        var organization = CreateNewOrganization(nameof(DeleteRoleAndPropertiesOrganization), 1);
        var organizationEntity = await _organizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);

        organization.Roles.Clear();
        organization.Properties.Clear();
        var updateEntity = await _organizationRegistry.UpdateOrganization(organization);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Empty(updateEntity.Roles);
        Assert.Empty(updateEntity.Properties);

        var success = await _organizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }
}