using Microsoft.EntityFrameworkCore;
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
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        
        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);

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
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);

        var organizationUpdate = CreateNewOrganization(nameof(OrganizationUpdate), 2);
        organizationUpdate.Properties.Add(new Organization.OrganizationProperty("test", "test"));
        var updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Equal(organizationUpdate.Name, updateEntity.Name);
        Assert.NotEqual(organization.Name, updateEntity.Name);

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
        var employeeEntity = await _authorizationRegistry.AddNewEmployeeToOrganization(organization.Identifier, employee);

        Assert.NotNull(employeeEntity);
        Assert.Equal(employee.EmployeeId, employeeEntity.EmployeeId);
        Assert.NotNull(employeeEntity.Properties);
        Assert.Equal(employee.Properties.Count, employeeEntity.Properties.Count);

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
        employeeUpdate.OrganizationId = organization.Identifier;
        employeeUpdate.Organization = organizationEntity;
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
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);
        Assert.Equal(1, organizationEntity.Employees.Count);

        employee.OrganizationId = "fail";
        employee.Organization = CreateNewOrganization("fail", 2);
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _authorizationRegistry.UpdateEmployee(employee));

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeUpdateUsingCreate()
    {
        var organization = CreateNewOrganization(nameof(EmployeeUpdateUsingCreate), 1);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);

        var employee = CreateNewEmployee(nameof(EmployeeUpdateUsingCreate), 1);
        await _authorizationRegistry.AddNewEmployeeToOrganization(organization.Identifier, employee);

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task ProductCrud()
    {
        var product = CreateNewProduct(nameof(ProductCrud), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

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
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var productUpdate = CreateNewProduct(nameof(ProductUpdate), 2);
        var updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(product.ProductId, updateEntity.ProductId);
        Assert.Equal(productUpdate.Name, updateEntity.Name);
        Assert.NotEqual(product.Name, updateEntity.Name);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task FeatureCrud()
    {
        var feature = CreateNewFeature(nameof(FeatureCrud), 1);
        var featureEntity = await _authorizationRegistry.CreateFeature(feature);

        Assert.NotNull(featureEntity);
        Assert.Equal(feature.FeatureId, featureEntity.FeatureId);
        Assert.NotNull(featureEntity.Properties);
        Assert.Equal(feature.Properties.Count, featureEntity.Properties.Count);

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
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);
        Assert.Equal(1, productEntity.Features.Count);

        feature.FeatureId = "fail";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _authorizationRegistry.UpdateFeature(feature));

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddNewFeatureToProduct()
    {
        var product = CreateNewProduct(nameof(AddNewFeatureToProduct), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var feature = CreateNewFeature(nameof(AddNewFeatureToProduct), 1);
        await _authorizationRegistry.AddNewFeatureToProduct(product.ProductId, feature);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddExistingFeatureToProduct()
    {
        var product = CreateNewProduct(nameof(AddExistingFeatureToProduct), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var feature = CreateNewFeature(nameof(AddExistingFeatureToProduct), 1);
        var featureEntity = await _authorizationRegistry.CreateFeature(feature);
        await _authorizationRegistry.AddExistingFeatureToProduct(product.ProductId, featureEntity.FeatureId);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task RemoveFeatureFromProduct()
    {
        var product = CreateNewProduct(nameof(RemoveFeatureFromProduct), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var feature = CreateNewFeature(nameof(RemoveFeatureFromProduct), 1);
        await _authorizationRegistry.AddNewFeatureToProduct(product.ProductId, feature);

        productEntity = await _authorizationRegistry.ReadProduct(product.ProductId);
        Assert.Equal(1, productEntity!.Features.Count);

        await _authorizationRegistry.RemoveFeatureFromProduct(product.ProductId, feature.FeatureId);

        productEntity = await _authorizationRegistry.ReadProduct(product.ProductId);
        Assert.Equal(0, productEntity!.Features.Count);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task CreateProductWithExistingFeatures()
    {
        var feature1 = CreateNewFeature(nameof(CreateProductWithExistingFeatures), 1);
        var featureEntity1 = await _authorizationRegistry.CreateFeature(feature1);
        var feature2 = CreateNewFeature(nameof(CreateProductWithExistingFeatures) + "2", 2);
        feature2.Properties.Clear();
        var featureEntity2 = await _authorizationRegistry.CreateFeature(feature2);

        var product = CreateNewProduct(nameof(CreateProductWithExistingFeatures), 1);
        var productEntity = await _authorizationRegistry.CreateProductWithExistingFeatures(product, new List<string>() { featureEntity1.FeatureId, featureEntity2.FeatureId });

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.Equal(2, productEntity.Features.Count);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }
}