using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
using System.Reflection;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class RepositoryTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAuthorizationRegistry _authorizationRegistry;

    public RepositoryTests()
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
        await (Task)runMigrations!.Invoke(_authorizationRegistry, null)!;
    }

    [Fact]
    public async Task OrganizationCrud()
    {
        var organization = TestData.CreateNewOrganization(nameof(OrganizationCrud), 1);
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

        var organizationUpdate = TestData.CreateNewOrganization(nameof(OrganizationCrud), 2);
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
        var organization = TestData.CreateNewOrganization(nameof(OrganizationUpdate), 1);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);

        var organizationUpdate = TestData.CreateNewOrganization(nameof(OrganizationUpdate), 2);
        organizationUpdate.Properties.Add(new Organization.OrganizationProperty("test", "test"));
        var updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(organization.Identifier, updateEntity.Identifier);
        Assert.Equal(organizationUpdate.Name, updateEntity.Name);
        Assert.NotEqual(organization.Name, updateEntity.Name);

        var employee = TestData.CreateNewEmployee(nameof(EmployeeUpdate), 1);
        organizationUpdate.Employees.Add(employee);
        updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.Single(updateEntity.Employees);

        updateEntity.Employees.First().GivenName = "updated-name";
        updateEntity.Employees.First().Properties.Add(new Employee.EmployeeProperty("test", "test"));
        updateEntity = await _authorizationRegistry.UpdateOrganization(organizationUpdate);

        Assert.Equal("updated-name", updateEntity.Employees.First().GivenName);
        Assert.Equal(3, updateEntity.Employees.First().Properties.Count);

        updateEntity.Employees.Clear();
        updateEntity.Properties.Clear();
        updateEntity = await _authorizationRegistry.UpdateOrganization(updateEntity);

        Assert.Empty(updateEntity.Employees);
        Assert.Empty(updateEntity.Properties);

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeCrud()
    {
        var organization = TestData.CreateNewOrganization(nameof(EmployeeCrud), 1);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);

        var employee = TestData.CreateNewEmployee(nameof(EmployeeCrud), 1);
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

        var employeeUpdate = TestData.CreateNewEmployee(nameof(EmployeeCrud), 2);
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
        var organization = TestData.CreateNewOrganization(nameof(EmployeeUpdate), 1);
        var employee = TestData.CreateNewEmployee(nameof(EmployeeUpdate), 1);
        organization.Employees.Add(employee);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);
        Assert.Single(organizationEntity.Employees);

        var employee2 = TestData.CreateNewEmployee(nameof(EmployeeUpdate), 2);
        organization.Employees.Add(employee2);
        var updateEntity = await _authorizationRegistry.UpdateEmployee(employee);

        Assert.Equal(2, organizationEntity.Employees.Count);

        organization.Employees.Remove(employee2);
        updateEntity = await _authorizationRegistry.UpdateEmployee(employee);

        Assert.Single(organizationEntity.Employees);

        organization.Employees.First().GivenName = "updated-name";
        organization.Employees.First().Properties.Add(new Employee.EmployeeProperty("test", "test"));
        updateEntity = await _authorizationRegistry.UpdateEmployee(employee);

        Assert.Equal("updated-name", organizationEntity.Employees.First().GivenName);
        Assert.Equal(3, organizationEntity.Employees.First().Properties.Count);

        updateEntity.Properties.Clear();
        updateEntity = await _authorizationRegistry.UpdateEmployee(updateEntity);

        Assert.Empty(updateEntity.Properties);

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task EmployeeUpdateUsingCreate()
    {
        var organization = TestData.CreateNewOrganization(nameof(EmployeeUpdateUsingCreate), 1);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);

        Assert.NotNull(organizationEntity);
        Assert.Equal(organization.Identifier, organizationEntity.Identifier);
        Assert.NotNull(organizationEntity.Properties);
        Assert.Equal(organization.Properties.Count, organizationEntity.Properties.Count);

        var employee = TestData.CreateNewEmployee(nameof(EmployeeUpdateUsingCreate), 1);
        await _authorizationRegistry.AddNewEmployeeToOrganization(organization.Identifier, employee);

        var success = await _authorizationRegistry.DeleteOrganization(organization.Identifier);
        Assert.True(success);
    }

    [Fact]
    public async Task ProductCrud()
    {
        var product = TestData.CreateNewProduct(nameof(ProductCrud), 1);
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

        var productUpdate = TestData.CreateNewProduct(nameof(ProductCrud), 2);
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
        var product = TestData.CreateNewProduct(nameof(ProductUpdate), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var productUpdate = TestData.CreateNewProduct(nameof(ProductUpdate), 2);
        var updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(product.ProductId, updateEntity.ProductId);
        Assert.Equal(productUpdate.Name, updateEntity.Name);
        Assert.NotEqual(product.Name, updateEntity.Name);

        var feature = TestData.CreateNewFeature(nameof(ProductUpdate), 1);
        productUpdate.Features.Add(feature);
        
        updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.Single(updateEntity.Features);

        updateEntity.Features.First().Name = "updated-name";
        productUpdate.Properties.Add(new Product.ProductProperty("test", "test"));
        updateEntity = await _authorizationRegistry.UpdateProduct(productUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal("updated-name", updateEntity.Features.First().Name);
        Assert.Equal(3, updateEntity.Properties.Count);

        updateEntity.Features.Clear();
        updateEntity.Properties.Clear();
        updateEntity = await _authorizationRegistry.UpdateProduct(updateEntity);

        Assert.Empty(updateEntity.Features);
        Assert.Empty(updateEntity.Properties);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task FeatureCrud()
    {
        var feature = TestData.CreateNewFeature(nameof(FeatureCrud), 1);
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

        var featureUpdate = TestData.CreateNewFeature(nameof(FeatureCrud), 2);
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
        var product = TestData.CreateNewProduct(nameof(FeatureUpdate), 1);
        var feature = TestData.CreateNewFeature(nameof(FeatureUpdate), 1);
        product.Features.Add(feature);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);
        Assert.Single(productEntity.Features);

        feature.FeatureId = "fail";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _authorizationRegistry.UpdateFeature(feature));

        var feature2 = TestData.CreateNewFeature(nameof(FeatureUpdate), 2);
        feature2.Properties.Add(new Feature.FeatureProperty("test", "test"));
        var updateEntity = await _authorizationRegistry.UpdateFeature(feature2);

        Assert.NotNull(updateEntity);
        Assert.Equal(feature2.FeatureId, updateEntity.FeatureId);
        Assert.Equal(3, updateEntity.Properties.Count);

        updateEntity.Properties.Clear();
        updateEntity = await _authorizationRegistry.UpdateFeature(updateEntity);

        Assert.Empty(updateEntity.Properties);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddNewFeatureToProduct()
    {
        var product = TestData.CreateNewProduct(nameof(AddNewFeatureToProduct), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var feature = TestData.CreateNewFeature(nameof(AddNewFeatureToProduct), 1);
        await _authorizationRegistry.AddNewFeatureToProduct(product.ProductId, feature);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddExistingFeatureToProduct()
    {
        var product = TestData.CreateNewProduct(nameof(AddExistingFeatureToProduct), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var feature = TestData.CreateNewFeature(nameof(AddExistingFeatureToProduct), 1);
        var featureEntity = await _authorizationRegistry.CreateFeature(feature);
        await _authorizationRegistry.AddExistingFeatureToProduct(product.ProductId, featureEntity.FeatureId);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task RemoveFeatureFromProduct()
    {
        var product = TestData.CreateNewProduct(nameof(RemoveFeatureFromProduct), 1);
        var productEntity = await _authorizationRegistry.CreateProduct(product);

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.NotNull(productEntity.Properties);
        Assert.Equal(product.Properties.Count, productEntity.Properties.Count);

        var feature = TestData.CreateNewFeature(nameof(RemoveFeatureFromProduct), 1);
        await _authorizationRegistry.AddNewFeatureToProduct(product.ProductId, feature);

        productEntity = await _authorizationRegistry.ReadProduct(product.ProductId);
        Assert.Single(productEntity!.Features);

        await _authorizationRegistry.RemoveFeatureFromProduct(product.ProductId, feature.FeatureId);

        productEntity = await _authorizationRegistry.ReadProduct(product.ProductId);
        Assert.Empty(productEntity!.Features);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }

    [Fact]
    public async Task CreateProductWithExistingFeatures()
    {
        var feature1 = TestData.CreateNewFeature(nameof(CreateProductWithExistingFeatures), 1);
        var featureEntity1 = await _authorizationRegistry.CreateFeature(feature1);
        var feature2 = TestData.CreateNewFeature(nameof(CreateProductWithExistingFeatures) + "2", 2);
        feature2.Properties.Clear();
        var featureEntity2 = await _authorizationRegistry.CreateFeature(feature2);

        var product = TestData.CreateNewProduct(nameof(CreateProductWithExistingFeatures), 1);
        var productEntity = await _authorizationRegistry.CreateProductWithExistingFeatures(product, new List<string>() { featureEntity1.FeatureId, featureEntity2.FeatureId });

        Assert.NotNull(productEntity);
        Assert.Equal(product.ProductId, productEntity.ProductId);
        Assert.Equal(2, productEntity.Features.Count);

        var success = await _authorizationRegistry.DeleteProduct(product.ProductId);
        Assert.True(success);
    }
}