using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.OrganizationRegistry.Extensions;
using System.Reflection;

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

        RunMigrations().Wait();
    }

    private async Task RunMigrations()
    {
        var runMigrations = _organizationRegistry.GetType().GetMethod("RunMigrations", BindingFlags.Public | BindingFlags.Instance);
        await (Task)runMigrations.Invoke(_organizationRegistry, null);
    }

    [Fact]
    public async Task CreateAndReadOrganization()
    {
        var id = Guid.NewGuid().ToString();
        var propId = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();
        var adherence = new Adherence("active", DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now));
        var roles = new List<OrganizationRole>();
        var properties = new List<Property>() { new Property("key", "value"), new Property("otherIdentifier", propId, true) };
        var organization = new Organization(id, name, adherence, roles, properties);

        var entity = await _organizationRegistry.CreateOrganization(organization);

        Assert.NotNull(entity);
        Assert.Equal(id, entity.Identifier);
        Assert.NotNull(entity.Properties);
        Assert.Equal(organization.Properties.Count, entity.Properties.Count);

        var readEntity = await _organizationRegistry.ReadOrganization(id);

        Assert.NotNull(readEntity);
        Assert.Equal(id, readEntity.Identifier);

        var readByPropIdEntity = await _organizationRegistry.ReadOrganization(propId);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(id, readByPropIdEntity.Identifier);

        var readByPropEntity = await _organizationRegistry.ReadOrganizations(propertyKey: "key", propertyValue: "value");

        Assert.NotNull(readByPropEntity);
        Assert.Single(readByPropEntity);
        Assert.Equal(id, readByPropEntity[0].Identifier);

        var readByNameEntity = await _organizationRegistry.ReadOrganizations(name: name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(id, readByNameEntity[0].Identifier);

        var newName = Guid.NewGuid().ToString();
        var organizationUpdate = new Organization(id, newName);
        var updateEntity = await _organizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(id, updateEntity.Identifier);

        readEntity = await _organizationRegistry.ReadOrganization(id);

        Assert.NotNull(readEntity);
        Assert.Equal(id, readEntity.Identifier);
        Assert.Equal(newName, readEntity.Name);

        var success = await _organizationRegistry.DeleteOrganization(id);
        Assert.True(success);
    }
}