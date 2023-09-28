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
        var properties = new List<OrganizationProperty>() { new OrganizationProperty("id", "key", "value") };
        var organization = new Organization("urn:organization:1", "Organization 1", properties);

        var entity = await _organizationRegistry.CreateOrganization(organization);
        var readEntity = await _organizationRegistry.ReadOrganization(entity.Identifier);

        Assert.NotNull(entity);
        Assert.Equal(organization.Identifier, entity.Identifier);
        Assert.Equal(organization.Name, entity.Name);
        Assert.NotNull(readEntity);
        Assert.Equal(organization.Identifier, readEntity.Identifier);
        Assert.Equal(organization.Name, readEntity.Name);
        Assert.NotNull(readEntity.Properties);
        Assert.Equal(organization.Properties.Count, readEntity.Properties.Count);
        Assert.Equal(organization.Properties[0].PropertyId, readEntity.Properties[0].PropertyId);
        Assert.Equal(organization.Properties[0].Key, readEntity.Properties[0].Key);
        Assert.Equal(organization.Properties[0].Value, readEntity.Properties[0].Value);
    }
}