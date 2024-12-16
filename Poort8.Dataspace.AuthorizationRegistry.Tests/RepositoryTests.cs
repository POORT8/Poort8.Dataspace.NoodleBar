using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
using static Poort8.Dataspace.AuthorizationRegistry.Entities.Organization;

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

        var factory = _serviceProvider.GetRequiredService<IDbContextFactory<AuthorizationContext>>();
        using var context = factory.CreateDbContext();
        context.Database.Migrate();

        _authorizationRegistry = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();
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
    public async Task ResourceGroupCrud()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(ResourceGroupCrud), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.NotNull(resourceGroupEntity.Properties);
        Assert.Equal(resourceGroup.Properties.Count, resourceGroupEntity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadResourceGroup(resourceGroup.ResourceGroupId);

        Assert.NotNull(readEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, readEntity.ResourceGroupId);

        var readByPropIdEntity = await _authorizationRegistry.ReadResourceGroup(resourceGroup.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, readByPropIdEntity.ResourceGroupId);

        var readByNameEntity = await _authorizationRegistry.ReadResourceGroups(name: resourceGroup.Name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, readByNameEntity[0].ResourceGroupId);

        var resourceGroupUpdate = TestData.CreateNewResourceGroup(nameof(ResourceGroupCrud), 2);
        var updateEntity = await _authorizationRegistry.UpdateResourceGroup(resourceGroupUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, updateEntity.ResourceGroupId);
        Assert.Equal(resourceGroupUpdate.Name, updateEntity.Name);
        Assert.NotEqual(resourceGroup.Name, updateEntity.Name);

        readEntity = await _authorizationRegistry.ReadResourceGroup(resourceGroup.ResourceGroupId);

        Assert.NotNull(readEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, readEntity.ResourceGroupId);
        Assert.Equal(resourceGroupUpdate.Name, updateEntity.Name);
        Assert.NotEqual(resourceGroup.Name, updateEntity.Name);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task ResourceGroupUpdate()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(ResourceGroupUpdate), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.NotNull(resourceGroupEntity.Properties);
        Assert.Equal(resourceGroup.Properties.Count, resourceGroupEntity.Properties.Count);

        var resourceGroupUpdate = TestData.CreateNewResourceGroup(nameof(ResourceGroupUpdate), 2);
        var updateEntity = await _authorizationRegistry.UpdateResourceGroup(resourceGroupUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, updateEntity.ResourceGroupId);
        Assert.Equal(resourceGroupUpdate.Name, updateEntity.Name);
        Assert.NotEqual(resourceGroup.Name, updateEntity.Name);

        var resource = TestData.CreateNewResource(nameof(ResourceGroupUpdate), 1);
        resourceGroupUpdate.Resources.Add(resource);

        updateEntity = await _authorizationRegistry.UpdateResourceGroup(resourceGroupUpdate);

        Assert.Single(updateEntity.Resources);

        updateEntity.Resources.First().Name = "updated-name";
        resourceGroupUpdate.Properties.Add(new ResourceGroup.ResourceGroupProperty("test", "test"));
        updateEntity = await _authorizationRegistry.UpdateResourceGroup(resourceGroupUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal("updated-name", updateEntity.Resources.First().Name);
        Assert.Equal(3, updateEntity.Properties.Count);

        updateEntity.Resources.Clear();
        updateEntity.Properties.Clear();
        updateEntity = await _authorizationRegistry.UpdateResourceGroup(updateEntity);

        Assert.Empty(updateEntity.Resources);
        Assert.Empty(updateEntity.Properties);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task ResourceCrud()
    {
        var resource = TestData.CreateNewResource(nameof(ResourceCrud), 1);
        var resourceEntity = await _authorizationRegistry.CreateResource(resource);

        Assert.NotNull(resourceEntity);
        Assert.Equal(resource.ResourceId, resourceEntity.ResourceId);
        Assert.NotNull(resourceEntity.Properties);
        Assert.Equal(resource.Properties.Count, resourceEntity.Properties.Count);

        var readEntity = await _authorizationRegistry.ReadResource(resource.ResourceId);

        Assert.NotNull(readEntity);
        Assert.Equal(resource.ResourceId, readEntity.ResourceId);

        var readByPropIdEntity = await _authorizationRegistry.ReadResource(resource.Properties.ToArray()[1].Value);

        Assert.NotNull(readByPropIdEntity);
        Assert.Equal(resource.ResourceId, readByPropIdEntity.ResourceId);

        var readByNameEntity = await _authorizationRegistry.ReadResources(name: resource.Name);

        Assert.NotNull(readByNameEntity);
        Assert.Single(readByNameEntity);
        Assert.Equal(resource.ResourceId, readByNameEntity[0].ResourceId);

        var resourceUpdate = TestData.CreateNewResource(nameof(ResourceCrud), 2);
        var updateEntity = await _authorizationRegistry.UpdateResource(resourceUpdate);

        Assert.NotNull(updateEntity);
        Assert.Equal(resource.ResourceId, updateEntity.ResourceId);
        Assert.Equal(resourceUpdate.Name, updateEntity.Name);
        Assert.NotEqual(resource.Name, updateEntity.Name);

        readEntity = await _authorizationRegistry.ReadResource(resource.ResourceId);

        Assert.NotNull(readEntity);
        Assert.Equal(resource.ResourceId, readEntity.ResourceId);
        Assert.Equal(resourceUpdate.Name, updateEntity.Name);
        Assert.NotEqual(resource.Name, updateEntity.Name);

        var success = await _authorizationRegistry.DeleteResource(resource.ResourceId);
        Assert.True(success);
    }

    [Fact]
    public async Task ResourceUpdate()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(ResourceUpdate), 1);
        var resource = TestData.CreateNewResource(nameof(ResourceUpdate), 1);
        resourceGroup.Resources.Add(resource);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.NotNull(resourceGroupEntity.Properties);
        Assert.Equal(resourceGroup.Properties.Count, resourceGroupEntity.Properties.Count);
        Assert.Single(resourceGroupEntity.Resources);

        resource.ResourceId = "fail";
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _authorizationRegistry.UpdateResource(resource));

        var resource2 = TestData.CreateNewResource(nameof(ResourceUpdate), 2);
        resource2.Properties.Add(new Resource.ResourceProperty("test", "test"));
        var updateEntity = await _authorizationRegistry.UpdateResource(resource2);

        Assert.NotNull(updateEntity);
        Assert.Equal(resource2.ResourceId, updateEntity.ResourceId);
        Assert.Equal(3, updateEntity.Properties.Count);

        updateEntity.Properties.Clear();
        updateEntity = await _authorizationRegistry.UpdateResource(updateEntity);

        Assert.Empty(updateEntity.Properties);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddNewResourceToResourceGroup()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(AddNewResourceToResourceGroup), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.NotNull(resourceGroupEntity.Properties);
        Assert.Equal(resourceGroup.Properties.Count, resourceGroupEntity.Properties.Count);

        var resource = TestData.CreateNewResource(nameof(AddNewResourceToResourceGroup), 1);
        await _authorizationRegistry.AddNewResourceToResourceGroup(resourceGroup.ResourceGroupId, resource);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task AddExistingResourceToResourceGroup()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(AddExistingResourceToResourceGroup), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.NotNull(resourceGroupEntity.Properties);
        Assert.Equal(resourceGroup.Properties.Count, resourceGroupEntity.Properties.Count);

        var resource = TestData.CreateNewResource(nameof(AddExistingResourceToResourceGroup), 1);
        var resourceEntity = await _authorizationRegistry.CreateResource(resource);
        await _authorizationRegistry.AddExistingResourceToResourceGroup(resourceGroup.ResourceGroupId, resourceEntity.ResourceId);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task RemoveResourceFromResourceGroup()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(RemoveResourceFromResourceGroup), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.NotNull(resourceGroupEntity.Properties);
        Assert.Equal(resourceGroup.Properties.Count, resourceGroupEntity.Properties.Count);

        var resource = TestData.CreateNewResource(nameof(RemoveResourceFromResourceGroup), 1);
        await _authorizationRegistry.AddNewResourceToResourceGroup(resourceGroup.ResourceGroupId, resource);

        resourceGroupEntity = await _authorizationRegistry.ReadResourceGroup(resourceGroup.ResourceGroupId);
        Assert.Single(resourceGroupEntity!.Resources);

        await _authorizationRegistry.RemoveResourceFromResourceGroup(resourceGroup.ResourceGroupId, resource.ResourceId);

        resourceGroupEntity = await _authorizationRegistry.ReadResourceGroup(resourceGroup.ResourceGroupId);
        Assert.Empty(resourceGroupEntity!.Resources);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task CreateResourceGroupWithExistingResources()
    {
        var resource1 = TestData.CreateNewResource(nameof(CreateResourceGroupWithExistingResources), 1);
        var resourceEntity1 = await _authorizationRegistry.CreateResource(resource1);
        var resource2 = TestData.CreateNewResource(nameof(CreateResourceGroupWithExistingResources) + "2", 2);
        resource2.Properties.Clear();
        var resourceEntity2 = await _authorizationRegistry.CreateResource(resource2);

        var resourceGroup = TestData.CreateNewResourceGroup(nameof(CreateResourceGroupWithExistingResources), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroupWithExistingResources(resourceGroup, new List<string>() { resourceEntity1.ResourceId, resourceEntity2.ResourceId });

        Assert.NotNull(resourceGroupEntity);
        Assert.Equal(resourceGroup.ResourceGroupId, resourceGroupEntity.ResourceGroupId);
        Assert.Equal(2, resourceGroupEntity.Resources.Count);

        var success = await _authorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
        Assert.True(success);
    }

    [Fact]
    public async Task SamePropertyKeyForTwoOrganizations()
    {
        var organization1 = TestData.CreateNewOrganization(nameof(SamePropertyKeyForTwoOrganizations), 1);
        organization1.Properties.Add(new OrganizationProperty("sharedIdentifier", "shared"));
        var organization1Entity = await _authorizationRegistry.CreateOrganization(organization1);
        Assert.NotNull(organization1Entity);

        var organization2 = TestData.CreateNewOrganization(nameof(SamePropertyKeyForTwoOrganizations) + "2", 1);
        organization2.Properties.Add(new OrganizationProperty("sharedIdentifier", "shared"));
        var organization2Entity = await _authorizationRegistry.CreateOrganization(organization2);
        Assert.NotNull(organization2Entity);
    }
}