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
        serviceCollection.AddAuthorizationRegistrySqlite(options => options.ConnectionString = $"Data Source={Guid.NewGuid()}.db");
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _authorizationRegistry = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();

        RunMigrations().Wait();
    }

    private async Task RunMigrations()
    {
        var runMigrations = _authorizationRegistry.GetType().GetMethod("RunMigrations", BindingFlags.Public | BindingFlags.Instance);
        await (Task)runMigrations.Invoke(_authorizationRegistry, null);
    }

    private static Organization CreateNewOrganization(string id, int index)
    {
        var properties = new List<Organization.OrganizationProperty>() { new Organization.OrganizationProperty("key", "value"), new Organization.OrganizationProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Organization($"{id}-id", $"{id}{index}-name", "url", "representative", "invoicingContact", properties);
    }

    private static Employee CreateNewEmployee(string id, int index)
    {
        var properties = new List<Employee.EmployeeProperty>() { new Employee.EmployeeProperty("key", "value"), new Employee.EmployeeProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Employee($"{id}-id", $"{id}{index}-name", "familyName", "telephone", "email", properties);
    }

    private static Product CreateNewProduct(string id, int index)
    {
        var properties = new List<Product.ProductProperty>() { new Product.ProductProperty("key", "value"), new Product.ProductProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Product($"{id}-id", $"{id}{index}-name", "description", "provider", "url", properties);
    }

    private static Feature CreateNewFeature(string id, int index)
    {
        var properties = new List<Feature.FeatureProperty>() { new Feature.FeatureProperty("key", "value"), new Feature.FeatureProperty("otherIdentifier", $"{id}{index}-otherId", true) };
        return new Feature($"{id}-id", $"{id}{index}-name", "description", properties);
    }

    [Fact]
    public async Task OrganizationCrud()
    {
        var organization = CreateNewOrganization(nameof(OrganizationCrud), 1);
        var entity = await _authorizationRegistry.CreateOrganization(organization);
        
        Assert.NotNull(entity);
        Assert.Equal(organization.Identifier, entity.Identifier);
        Assert.NotNull(entity.Properties);
        Assert.Equal(organization.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadOrganization(organization.Identifier);

        Assert.NotNull(readEntity);
        Assert.Equal(organization.Identifier, readEntity.Identifier);

        var readByPropIdEntity = await _authorizationRegistry.ReadOrganization(organization.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(organization.Identifier, readByPropIdEntity.Identifier);

        var readByPropEntity = await _authorizationRegistry.ReadOrganizations(propertyKey: "key", propertyValue: "value");

        Assert.NotNull(readByPropEntity);
        Assert.Single(readByPropEntity);
        Assert.Equal(organization.Identifier, readByPropEntity[0].Identifier);

        var readByNameEntity = await _authorizationRegistry.ReadOrganizations(name: organization.Name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(organization.Identifier, readByNameEntity[0].Identifier);

        var organizationUpdate = CreateNewOrganization(nameof(OrganizationCrud), 2);
        var updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Equal(organizationUpdate.Name, updateEntity.Name);
        Assert.NotEqual(organization.Name, updateEntity.Name);

        readEntity = await _authorizationRegistry.ReadOrganization(organization.Identifier);

        Assert.NotNull(readEntity);
        Assert.Equal(organization.Identifier, readEntity.Identifier);
        Assert.Equal(organizationUpdate.Name, updateEntity.Name);
        Assert.NotEqual(organization.Name, updateEntity.Name);

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task OrganizationUpdate()
    {
        var organization = CreateNewOrganization(nameof(OrganizationUpdate), 1);
        var entity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(entity);
        Assert.Equal(organization.Identifier, entity.Identifier);
        Assert.NotNull(entity.Properties);
        Assert.Equal(organization.Properties.Count, entity.Properties.Count);

        var organizationUpdate = CreateNewOrganization(nameof(OrganizationUpdate), 2);
        var updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Equal(organizationUpdate.Name, updateEntity.Name);
        Assert.NotEqual(organization.Name, updateEntity.Name);

        organizationUpdate.Employees.Add(CreateNewEmployee(nameof(OrganizationUpdate), 1));
        await Assert.ThrowsAsync<Exception>(async () => await _authorizationRegistry.UpdateOrganization(organizationUpdate));

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeCrud()
    {
        var organization = CreateNewOrganization(nameof(EmployeeCrud), 1);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);

        var employee = CreateNewEmployee(nameof(EmployeeCrud), 1);
        var entity = await _authorizationRegistry.AddEmployeeToOrganization(organization.Identifier, employee);

        Assert.NotNull(entity);
        Assert.Equal(employee.EmployeeId, entity.EmployeeId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(employee.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadEmployee(employee.EmployeeId);

        Assert.NotNull(readEntity);
        Assert.Equal(employee.EmployeeId, readEntity.EmployeeId);
        
        var readByPropIdEntity = await _authorizationRegistry.ReadEmployee(employee.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(employee.EmployeeId, readByPropIdEntity.EmployeeId);

        var readByOrganizationIdEntity = await _authorizationRegistry.ReadEmployees(organizationId: organization.Identifier);

        Assert.NotNull(readByOrganizationIdEntity);
        Assert.Single(readByOrganizationIdEntity);
        Assert.Equal(employee.EmployeeId, readByOrganizationIdEntity[0].EmployeeId);

        var employeeUpdate = CreateNewEmployee(nameof(EmployeeCrud), 2);
        var updateEntity = await _authorizationRegistry.UpdateEmployee(employeeUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(employee.EmployeeId, updateEntity.EmployeeId);
        Assert.Equal(employeeUpdate.GivenName, updateEntity.GivenName);
        Assert.NotEqual(employee.GivenName, updateEntity.GivenName);

        readEntity = await _authorizationRegistry.ReadEmployee(employee.EmployeeId);

        Assert.NotNull(readEntity);
        Assert.Equal(employee.EmployeeId, readEntity.EmployeeId);
        Assert.Equal(employeeUpdate.GivenName, updateEntity.GivenName);
        Assert.NotEqual(employee.GivenName, updateEntity.GivenName);

        var success = await _authorizationRegistry.DeleteEmployee(employee.EmployeeId);
        Assert.True(success);

        organizationEntity = await _authorizationRegistry.ReadOrganization(organization.Identifier);

        Assert.NotNull(organizationEntity);
        Assert.Empty(organizationEntity.Employees);

        success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeUpdate()
    {
        var organization = CreateNewOrganization(nameof(EmployeeUpdate), 1);
        var employee = CreateNewEmployee(nameof(EmployeeUpdate), 1);
        organization.Employees.Add(employee);
        var entity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(entity);
        Assert.Equal(organization.Identifier, entity.Identifier);
        Assert.NotNull(entity.Properties);
        Assert.Equal(organization.Properties.Count, entity.Properties.Count);
        Assert.Equal(1, entity.Employees.Count);

        employee.OrganizationId = "fail";
        await Assert.ThrowsAsync<Exception>(async () => await _authorizationRegistry.UpdateEmployee(employee));

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeUpdateUsingCreate()
    {
        var organization = CreateNewOrganization(nameof(EmployeeUpdateUsingCreate), 1);
        var entity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(entity);
        Assert.Equal(organization.Identifier, entity.Identifier);
        Assert.NotNull(entity.Properties);
        Assert.Equal(organization.Properties.Count, entity.Properties.Count);

        var employee = CreateNewEmployee(nameof(EmployeeUpdateUsingCreate), 1);
        await _authorizationRegistry.AddEmployeeToOrganization(organization.Identifier, employee);

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task ProductCrud()
    {
        var product = CreateNewProduct(nameof(ProductCrud), 1);
        var entity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(entity);
        Assert.Equal(product.ProductId, entity.ProductId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(product.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadProduct(product.ProductId);

        Assert.NotNull(readEntity);
        Assert.Equal(product.ProductId, readEntity.ProductId);

        var readByPropIdEntity = await _authorizationRegistry.ReadProduct(product.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(product.ProductId, readByPropIdEntity.ProductId);

        var readByNameEntity = await _authorizationRegistry.ReadProducts(name: product.Name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(product.ProductId, readByNameEntity[0].ProductId);

        var productUpdate = CreateNewProduct(nameof(ProductCrud), 2);
        var updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(product.ProductId, updateEntity.ProductId);
        Assert.Equal(productUpdate.Name, updateEntity.Name);
        Assert.NotEqual(product.Name, updateEntity.Name);

        readEntity = await _authorizationRegistry.ReadProduct(product.ProductId);

        Assert.NotNull(readEntity);
        Assert.Equal(product.ProductId, readEntity.ProductId);
        Assert.Equal(productUpdate.Name, updateEntity.Name);
        Assert.NotEqual(product.Name, updateEntity.Name);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task ProductUpdate()
    {
        var product = CreateNewProduct(nameof(ProductUpdate), 1);
        var entity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(entity);
        Assert.Equal(product.ProductId, entity.ProductId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(product.Properties.Count, entity.Properties.Count);

        var productUpdate = CreateNewProduct(nameof(ProductUpdate), 2);
        var updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(product.ProductId, updateEntity.ProductId);
        Assert.Equal(productUpdate.Name, updateEntity.Name);
        Assert.NotEqual(product.Name, updateEntity.Name);

        productUpdate.Features.Add(CreateNewFeature(nameof(ProductUpdate), 1));
        await Assert.ThrowsAsync<Exception>(async () => await _authorizationRegistry.UpdateProduct(productUpdate));

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task FeatureCrud()
    {
        var feature = CreateNewFeature(nameof(FeatureCrud), 1);
        var entity = await _authorizationRegistry.CreateFeature(feature);

        Assert.NotNull(entity);
        Assert.Equal(feature.FeatureId, entity.FeatureId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(feature.Properties.Count, entity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadFeature(feature.FeatureId);

        Assert.NotNull(readEntity);
        Assert.Equal(feature.FeatureId, readEntity.FeatureId);

        var readByPropIdEntity = await _authorizationRegistry.ReadFeature(feature.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(feature.FeatureId, readByPropIdEntity.FeatureId);

        var readByNameEntity = await _authorizationRegistry.ReadFeatures(name: feature.Name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(feature.FeatureId, readByNameEntity[0].FeatureId);

        var featureUpdate = CreateNewFeature(nameof(FeatureCrud), 2);
        var updateEntity = await _authorizationRegistry.UpdateFeature(featureUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(feature.FeatureId, updateEntity.FeatureId);
        Assert.Equal(featureUpdate.Name, updateEntity.Name);
        Assert.NotEqual(feature.Name, updateEntity.Name);

        readEntity = await _authorizationRegistry.ReadFeature(feature.FeatureId);

        Assert.NotNull(readEntity);
        Assert.Equal(feature.FeatureId, readEntity.FeatureId);
        Assert.Equal(featureUpdate.Name, updateEntity.Name);
        Assert.NotEqual(feature.Name, updateEntity.Name);

        var success = await _authorizationRegistry.DeleteFeature(feature.FeatureId);
        Assert.True(success);
    }

    [Fact]
    public async Task FeatureUpdate()
    {
        var product = CreateNewProduct(nameof(FeatureUpdate), 1);
        var feature = CreateNewFeature(nameof(FeatureUpdate), 1);
        product.Features.Add(feature);
        var entity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(entity);
        Assert.Equal(product.ProductId, entity.ProductId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(product.Properties.Count, entity.Properties.Count);
        Assert.Equal(1, entity.Features.Count);

        feature.FeatureId = "fail";
        await Assert.ThrowsAsync<Exception>(async () => await _authorizationRegistry.UpdateFeature(feature));

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task FeatureUpdateUsingCreate()
    {
        var product = CreateNewProduct(nameof(FeatureUpdateUsingCreate), 1);
        var entity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(entity);
        Assert.Equal(product.ProductId, entity.ProductId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(product.Properties.Count, entity.Properties.Count);

        var feature = CreateNewFeature(nameof(FeatureUpdateUsingCreate), 1);
        await _authorizationRegistry.AddFeatureToProduct(product.ProductId, feature);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddFeatureToAnotherProduct()
    {
        var product = CreateNewProduct(nameof(AddFeatureToAnotherProduct), 1);
        var feature = CreateNewFeature(nameof(AddFeatureToAnotherProduct), 1);
        product.Features.Add(feature);
        var entity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(entity);
        Assert.Equal(product.ProductId, entity.ProductId);
        Assert.NotNull(entity.Properties);
        Assert.Equal(product.Properties.Count, entity.Properties.Count);
        Assert.Equal(1, entity.Features.Count);

        var product2 = CreateNewProduct(nameof(AddFeatureToAnotherProduct), 2);
        product2.Features.Add(feature);
        var entity2 = await _authorizationRegistry.CreateProduct(product2);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);

        success = await _authorizationRegistry.DeleteProduct(product2.ProductId);
        Assert.True(success);
    }
}