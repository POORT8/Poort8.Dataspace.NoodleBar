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
        serviceCollection.AddOrganizationRegistrySqlite(options => { });
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _organizationRegistry = _serviceProvider.GetRequiredService<IOrganizationRegistry>();
    }

    [Fact]
    public async Task CreateAndReadOrganization()
    {
        var adherence = new Adherence("active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now));
        var roles = new List<OrganizationRole>();
        var properties = new List<OrganizationProperty>() { new OrganizationProperty("key", "value") };
        var organization = new Organization("urn:organization:1", "Organization 1", adherence, roles, properties);

        var entity = await _organizationRegistry.CreateOrganization(organization);
        var readEntity = await _organizationRegistry.ReadOrganization(entity.Identifier);

        Assert.NotNull(entity);
        Assert.Equal(organization.Identifier, entity.Identifier);
        Assert.Equal(organization.Name, entity.Name);
        Assert.NotNull(readEntity);
        Assert.Equal(organization.Identifier, readEntity.Identifier);
        Assert.Equal(organization.Name, readEntity.Name);
        Assert.NotNull(readEntity.Properties);
        Assert.Equal(organization.Properties.Count(), readEntity.Properties.Count());
        Assert.Equal(organization.Properties.First().PropertyId, readEntity.Properties.First().PropertyId);
        Assert.Equal(organization.Properties.First().Key, readEntity.Properties.First().Key);
        Assert.Equal(organization.Properties.First().Value, readEntity.Properties.First().Value);

        var success = await _organizationRegistry.DeleteOrganization(entity.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task ReadQueryParameters()
    {
        var adherence = new Adherence("active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now));
        var roles = new List<OrganizationRole>();
        var properties = new List<OrganizationProperty>() { new OrganizationProperty("key", "value") };
        var organization = new Organization("A", "Organization A", adherence, roles, properties);

        await _organizationRegistry.CreateOrganization(organization);

        var adherence2 = new Adherence("active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now));
        var roles2 = new List<OrganizationRole>();
        var properties2 = new List<OrganizationProperty>() { new OrganizationProperty("keyB", "valueB") };
        var organization2 = new Organization("B", "Organization B", adherence2, roles2, properties2);

        await _organizationRegistry.CreateOrganization(organization2);

        var organizations = await _organizationRegistry.ReadOrganizations("Organization A");
        Assert.Single(organizations);
        Assert.Equal("Organization A", organizations.First().Name);

        organizations = await _organizationRegistry.ReadOrganizations(adherenceStatus: "active");
        Assert.Equal(2, organizations.Count());

        organizations = await _organizationRegistry.ReadOrganizations(propertyKey: "key", propertyValue: "value");
        Assert.Single(organizations);

        await Assert.ThrowsAsync<ArgumentException>(() => _organizationRegistry.ReadOrganizations(propertyKey: "key"));
        await Assert.ThrowsAsync<ArgumentException>(() => _organizationRegistry.ReadOrganizations(propertyValue: "value"));

        var success = await _organizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);

        success = await _organizationRegistry.DeleteOrganization(organization2.Identifier);
        Assert.True(success);
    }
}