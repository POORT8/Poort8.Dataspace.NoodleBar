using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using System.Reflection;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;

public class AuthorizationRegistryTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAuthorizationRegistry _authorizationRegistry;

    public AuthorizationRegistryTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAuthorizationRegistrySqlite(options => { });
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _authorizationRegistry = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();

        RunMigrations().Wait();
    }

    private async Task RunMigrations()
    {
        var runMigrations = _authorizationRegistry.GetType().GetMethod("RunMigrations", BindingFlags.Public | BindingFlags.Instance);
        await (Task)runMigrations.Invoke(_authorizationRegistry, null);
    }

    [Fact]
    public async Task OrganizationCrud()
    {
        var id = Guid.NewGuid().ToString();
        var propId = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();
        var properties = new List<Property>() { new Property("key", "value"), new Property("otherIdentifier", propId, true) };
        var organization = new Organization(id, name, "url", "representative", "invoicingContact", properties);

        var entity = await _authorizationRegistry.CreateOrganization(organization);
        
        Assert.NotNull(entity);
        Assert.Equal(id, entity.Identifier);
        Assert.NotNull(entity.Properties);
        Assert.Equal(organization.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadOrganization(id);

        Assert.NotNull(readEntity);
        Assert.Equal(id, readEntity.Identifier);

        var readByPropIdEntity = await _authorizationRegistry.ReadOrganization(propId);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(id, readByPropIdEntity.Identifier);

        var readByNameEntity = await _authorizationRegistry.ReadOrganizations(name: name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(id, readByNameEntity[0].Identifier);

        var newName = Guid.NewGuid().ToString();
        var organizationUpdate = new Organization(id, newName, "url", "representative", "invoicingContact");
        var updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(id, updateEntity.Identifier);

        readEntity = await _authorizationRegistry.ReadOrganization(id);

        Assert.NotNull(readEntity);
        Assert.Equal(id, readEntity.Identifier);
        Assert.Equal(newName, readEntity.Name);

        var success = await _authorizationRegistry.DeleteOrganization(id);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeCrud()
    {
        var organizationId = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();
        var organization = new Organization(organizationId, name, "url", "representative", "invoicingContact");

        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);

        var employeeId = Guid.NewGuid().ToString();
        var propId = Guid.NewGuid().ToString();
        var properties = new List<Property>() { new Property("key", "value"), new Property("otherIdentifier", propId, true) };
        var employee = new Employee(employeeId, "givenName", "familyName", "telephone", "email", properties);

        var entity = await _authorizationRegistry.AddEmployee(organizationId, employee);

        Assert.NotNull(entity);
        Assert.Equal(employeeId, entity.EmployeeId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(employee.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadEmployee(employeeId);

        Assert.NotNull(readEntity);
        Assert.Equal(employeeId, readEntity.EmployeeId);
        
        var readByPropIdEntity = await _authorizationRegistry.ReadEmployee(propId);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(employeeId, readByPropIdEntity.EmployeeId);

        var readByOrganizationIdEntity = await _authorizationRegistry.ReadEmployees(organizationId: organizationId);

        Assert.NotNull(readByOrganizationIdEntity);
        Assert.Single(readByOrganizationIdEntity);
        Assert.Equal(employeeId, readByOrganizationIdEntity[0].EmployeeId);

        var newName = Guid.NewGuid().ToString();
        var employeeUpdate = new Employee(employeeId, newName, "familyName", "telephone", "email");
        var updateEntity = await _authorizationRegistry.UpdateEmployee(employeeUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(employeeId, updateEntity.EmployeeId);

        readEntity = await _authorizationRegistry.ReadEmployee(employeeId);

        Assert.NotNull(readEntity);
        Assert.Equal(employeeId, readEntity.EmployeeId);
        Assert.Equal(newName, readEntity.GivenName);

        var success = await _authorizationRegistry.DeleteEmployee(employeeId);
        Assert.True(success);

        organizationEntity = await _authorizationRegistry.ReadOrganization(organizationId);

        Assert.NotNull(organizationEntity);
        Assert.Empty(organizationEntity.Employees);
    }

    [Fact]
    public async Task ProductCrud()
    {
        var id = Guid.NewGuid().ToString();
        var propId = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();
        var properties = new List<Property>() { new Property("key", "value"), new Property("otherIdentifier", propId, true) };
        var product = new Product(id, name, "description", "provider", "url", properties);

        var entity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(entity);
        Assert.Equal(id, entity.ProductId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(product.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadProduct(id);

        Assert.NotNull(readEntity);
        Assert.Equal(id, readEntity.ProductId);

        var readByPropIdEntity = await _authorizationRegistry.ReadProduct(propId);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(id, readByPropIdEntity.ProductId);

        var readByNameEntity = await _authorizationRegistry.ReadProducts(name: name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(id, readByNameEntity[0].ProductId);

        var newName = Guid.NewGuid().ToString();
        var productUpdate = new Product(id, newName, "description", "provider", "url");
        var updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(id, updateEntity.ProductId);

        readEntity = await _authorizationRegistry.ReadProduct(id);

        Assert.NotNull(readEntity);
        Assert.Equal(id, readEntity.ProductId);
        Assert.Equal(newName, readEntity.Name);

        var success = await _authorizationRegistry.DeleteProduct(id);
        Assert.True(success);
    }

    [Fact]
    public async Task FeatureCrud()
    {
        var productId = Guid.NewGuid().ToString();
        var name = Guid.NewGuid().ToString();
        var product = new Product(productId, name, "description", "provider", "url");

        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);

        var featureId = Guid.NewGuid().ToString();
        var propId = Guid.NewGuid().ToString();
        var properties = new List<Property>() { new Property("key", "value"), new Property("otherIdentifier", propId, true) };
        var feature = new Feature(featureId, "name", "description", properties);

        var entity = await _authorizationRegistry.AddFeature(productId, feature);

        Assert.NotNull(entity);
        Assert.Equal(featureId, entity.FeatureId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(feature.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadFeature(featureId);

        Assert.NotNull(readEntity);
        Assert.Equal(featureId, readEntity.FeatureId);

        var readByPropIdEntity = await _authorizationRegistry.ReadFeature(propId);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(featureId, readByPropIdEntity.FeatureId);

        var readByNameEntity = await _authorizationRegistry.ReadFeatures(name: "name");

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(featureId, readByNameEntity[0].FeatureId);

        var newName = Guid.NewGuid().ToString();
        var featureUpdate = new Feature(featureId, newName, "description");
        var updateEntity = await _authorizationRegistry.UpdateFeature(featureUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(featureId, updateEntity.FeatureId);

        readEntity = await _authorizationRegistry.ReadFeature(featureId);

        Assert.NotNull(readEntity);
        Assert.Equal(featureId, readEntity.FeatureId);
        Assert.Equal(newName, readEntity.Name);

        var success = await _authorizationRegistry.DeleteFeature(featureId);
        Assert.True(success);

        productEntity = await _authorizationRegistry.ReadProduct(productId);

        Assert.NotNull(productEntity);
        Assert.Empty(productEntity.Features);
    }
}