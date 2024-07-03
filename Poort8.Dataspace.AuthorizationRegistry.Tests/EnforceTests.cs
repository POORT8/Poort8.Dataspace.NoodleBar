using Casbin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Fakes;
using System.Reflection;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class EnforceTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAuthorizationRegistry _authorizationRegistry;
    private static readonly string[] expected = ["emp", "org", "default"];

    public EnforceTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAuthorizationRegistrySqlite(options => { });

        serviceCollection.Replace(new ServiceDescriptor(typeof(IRepository), typeof(FakeRepository), ServiceLifetime.Singleton));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _authorizationRegistry = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();
    }

    [Fact]
    public async Task TestModel()
    {
        IEnforcer? enforcer = _authorizationRegistry
            .GetType()
            .GetField("_enforcer", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_authorizationRegistry) as IEnforcer;
        Assert.NotNull(enforcer);

        var success = await enforcer.AddNamedGroupingPolicyAsync("subjectGroup", "emp", "org", "default");
        Assert.True(success);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = "org";
        var policyValuesMethod = policy.GetType().GetMethod("ToPolicyValues");
        var policyValues = policyValuesMethod!.Invoke(policy, null) as string[];
        success = await enforcer.AddPolicyAsync(policyValues);
        Assert.True(success);

        var enforcerPolicy = enforcer.GetPolicy();
        Assert.Single(enforcerPolicy);
        Assert.Equal(policyValues, enforcerPolicy.First());

        var enforcerGroup = enforcer.GetNamedGroupingPolicy("subjectGroup");
        Assert.Single(enforcerGroup);
        Assert.Equal(expected, enforcerGroup.First());

        var now = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var allowed = await enforcer.EnforceAsync(policy.UseCase, now, "org", policy.ResourceId, policy.Action);
        Assert.True(allowed);

        allowed = await enforcer.EnforceAsync(policy.UseCase, now, "emp", policy.ResourceId, policy.Action);
        Assert.True(allowed);
    }

    [Fact]
    public async Task CreateAndEnforcePolicy()
    {
        var policyEntity = await _authorizationRegistry.CreatePolicy(TestData.CreateNewPolicy());
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.True(allowed);
    }

    [Fact]
    public async Task CreateDuplicatePolicies()
    {
        var policy = TestData.CreateNewPolicy();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        policy.Action = "exception";
        await Assert.ThrowsAsync<Exception>(async () => await _authorizationRegistry.CreatePolicy(policy));
    }

    [Fact]
    public async Task EnforceNotBefore()
    {
        var policy = TestData.CreateNewPolicy();
        policy.NotBefore = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EnforceExpiration()
    {
        var policy = TestData.CreateNewPolicy();
        policy.NotBefore = DateTimeOffset.Now.AddDays(-2).ToUnixTimeSeconds();
        policy.Expiration = DateTimeOffset.Now.AddDays(-1).ToUnixTimeSeconds();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EnforceActionNotAllowed()
    {
        var policy = TestData.CreateNewPolicy();
        policy.Action = "fail";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EnforceOtherUseCase()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "other";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);

        allowed = await _authorizationRegistry.Enforce("subject", "resource", "action", useCase: "other");
        Assert.True(allowed);
    }

    [Fact]
    public async Task EnforceHwct()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "hwct";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);

        allowed = await _authorizationRegistry.Enforce("subject", "resource", "action", useCase: "hwct");
        Assert.True(allowed);
    }

    [Fact]
    public async Task UpdatePolicy()
    {
        var policy = TestData.CreateNewPolicy();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        policyEntity.SubjectId = "updated-subject";
        var updatedPolicyEntity = await _authorizationRegistry.UpdatePolicy(policyEntity);
        Assert.NotNull(updatedPolicyEntity);
        Assert.Equal("updated-subject", updatedPolicyEntity.SubjectId);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);

        allowed = await _authorizationRegistry.Enforce(policyEntity.SubjectId, "resource", "action");
        Assert.True(allowed);
    }

    [Fact]
    public async Task DeletePolicy()
    {
        var policy = TestData.CreateNewPolicy();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.True(allowed);

        var deleted = await _authorizationRegistry.DeletePolicy(policyEntity.PolicyId);
        Assert.True(deleted);

        allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task ExplainedEnforce()
    {
        var policyEntity = await _authorizationRegistry.CreatePolicy(TestData.CreateNewPolicy());
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("subject", "resource", "action");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task ExplainedEnforceOtherUseCase()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "other";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("subject", "resource", "action");
        Assert.False(allowed);
        Assert.Empty(explainPolicy);

        (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("subject", "resource", "action", useCase: "other");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task ExplainedEnforceIshare()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "ishare";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute", "other");
        Assert.False(allowed);
        Assert.Empty(explainPolicy);

        (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task ExplainedEnforceGir()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "gir";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute", "test", useCase: "gir");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task OrganizationEmployeeEnforce()
    {
        var organization = TestData.CreateNewOrganization(nameof(OrganizationEmployeeEnforce), 1);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organizationEntity.Identifier;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce(organizationEntity.Identifier, "resource", "action");
        Assert.True(allowed);

        var employee = TestData.CreateNewEmployee(nameof(OrganizationEmployeeEnforce), 1);
        var employeeEntity = await _authorizationRegistry.AddNewEmployeeToOrganization(organizationEntity.Identifier, employee);
        Assert.NotNull(employeeEntity);

        allowed = await _authorizationRegistry.Enforce(employeeEntity.EmployeeId, "resource", "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteEmployee(employeeEntity.EmployeeId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce(employeeEntity.EmployeeId, "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task OrganizationWithEmployeeEnforce()
    {
        var organization = TestData.CreateNewOrganization(nameof(OrganizationWithEmployeeEnforce), 1);
        var employee = TestData.CreateNewEmployee(nameof(OrganizationWithEmployeeEnforce), 1);
        organization.Employees.Add(employee);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organizationEntity.Identifier;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce(organizationEntity.Identifier, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteEmployee(employee.EmployeeId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task ResourceGroupResourceEnforce()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(ResourceGroupResourceEnforce), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);
        Assert.NotNull(resourceGroupEntity);

        var policy = TestData.CreateNewPolicy();
        policy.ResourceId = resourceGroupEntity.ResourceGroupId;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", resourceGroupEntity.ResourceGroupId, "action");
        Assert.True(allowed);

        var resource = TestData.CreateNewResource(nameof(ResourceGroupResourceEnforce), 1);
        var resourceEntity = await _authorizationRegistry.AddNewResourceToResourceGroup(resourceGroupEntity.ResourceGroupId, resource);
        Assert.NotNull(resourceEntity);

        allowed = await _authorizationRegistry.Enforce("subject", resourceEntity.ResourceId, "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteResource(resourceEntity.ResourceId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce("subject", resourceEntity.ResourceId, "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task ResourceGroupWithResourceEnforce()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(ResourceGroupWithResourceEnforce), 1);
        var resource = TestData.CreateNewResource(nameof(ResourceGroupWithResourceEnforce), 1);
        resourceGroup.Resources.Add(resource);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);
        Assert.NotNull(resourceGroupEntity);

        var policy = TestData.CreateNewPolicy();
        policy.ResourceId = resourceGroupEntity.ResourceGroupId;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", resourceGroupEntity.ResourceGroupId, "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("subject", resource.ResourceId, "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteResource(resource.ResourceId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce("subject", resource.ResourceId, "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task CreateResourceGroupWithExistingResources()
    {
        var resource = TestData.CreateNewResource(nameof(CreateResourceGroupWithExistingResources), 1);
        var resourceEntity = await _authorizationRegistry.CreateResource(resource);
        Assert.NotNull(resourceEntity);

        var resourceGroup = TestData.CreateNewResourceGroup(nameof(CreateResourceGroupWithExistingResources), 1);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroupWithExistingResources(
            resourceGroup,
            new List<string> { resourceEntity.ResourceId });
        Assert.NotNull(resourceGroupEntity);

        var policy = TestData.CreateNewPolicy();
        policy.ResourceId = resourceGroupEntity.ResourceGroupId;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", resourceGroupEntity.ResourceGroupId, "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("subject", resourceEntity.ResourceId, "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteResource(resourceEntity.ResourceId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce("subject", resourceEntity.ResourceId, "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task OrganizationWithOtherIdEnforce()
    {
        var organization = TestData.CreateNewOrganization(nameof(OrganizationWithEmployeeEnforce), 1);
        var employee = TestData.CreateNewEmployee(nameof(OrganizationWithEmployeeEnforce), 1);
        organization.Employees.Add(employee);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organizationEntity.Identifier;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce(organizationEntity.Identifier, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.True(allowed);

        var otherId = organizationEntity.Properties.First(p => p.Key == "otherIdentifier").Value;
        allowed = await _authorizationRegistry.Enforce(otherId, "resource", "action");
        Assert.True(allowed);

        otherId = organization.Employees.First().Properties.First(p => p.Key == "otherIdentifier").Value;
        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteEmployee(employee.EmployeeId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task ResourceGroupWithOtherIdEnforce()
    {
        var resourceGroup = TestData.CreateNewResourceGroup(nameof(ResourceGroupWithOtherIdEnforce), 1);
        var resource = TestData.CreateNewResource(nameof(ResourceGroupWithOtherIdEnforce), 1);
        resourceGroup.Resources.Add(resource);
        var resourceGroupEntity = await _authorizationRegistry.CreateResourceGroup(resourceGroup);
        Assert.NotNull(resourceGroupEntity);

        var policy = TestData.CreateNewPolicy();
        policy.ResourceId = resourceGroupEntity.ResourceGroupId;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", resourceGroupEntity.ResourceGroupId, "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("subject", resource.ResourceId, "action");
        Assert.True(allowed);

        var otherId = resourceGroupEntity.Properties.First(p => p.Key == "otherIdentifier").Value;
        allowed = await _authorizationRegistry.Enforce("subject", otherId, "action");
        Assert.True(allowed);

        otherId = resourceGroup.Resources.First().Properties.First(p => p.Key == "otherIdentifier").Value;
        allowed = await _authorizationRegistry.Enforce("subject", otherId, "action");
        Assert.True(allowed);

        var success = await _authorizationRegistry.DeleteResource(resource.ResourceId);
        Assert.True(success);

        allowed = await _authorizationRegistry.Enforce(resource.ResourceId, "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EmployeeEmailTelephoneIdentifier()
    {
        var organization = TestData.CreateNewOrganization(nameof(OrganizationWithEmployeeEnforce), 1);
        var employee = TestData.CreateNewEmployee(nameof(OrganizationWithEmployeeEnforce), 1);
        employee.Email = "email@domain.com";
        employee.Telephone = "1234567890";
        organization.Employees.Add(employee);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organizationEntity.Identifier;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce(organizationEntity.Identifier, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.Email, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.Telephone, "resource", "action");
        Assert.True(allowed);

        organization.Employees.First().Email = "updated-email@domain.com";
        organization.Employees.First().Telephone = "updated-1234567890";
        var organizationUpdateEntity = await _authorizationRegistry.UpdateOrganization(organization);
        Assert.NotNull(organizationUpdateEntity);

        allowed = await _authorizationRegistry.Enforce("updated-email@domain.com", "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("updated-1234567890", "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("email@domain.com", "resource", "action");
        Assert.False(allowed);

        allowed = await _authorizationRegistry.Enforce("1234567890", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EmployeePropertyUpdate()
    {
        var organization = TestData.CreateNewOrganization(nameof(EmployeePropertyUpdate), 1);
        var employee = TestData.CreateNewEmployee(nameof(EmployeePropertyUpdate), 1);
        employee.Email = "email@domain.com";
        employee.Telephone = "1234567890";
        organization.Employees.Add(employee);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organizationEntity.Identifier;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce(organizationEntity.Identifier, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.Email, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.Telephone, "resource", "action");
        Assert.True(allowed);

        employee.Email = "updated-email@domain.com";
        employee.Telephone = "updated-1234567890";
        employee.Properties.First(p => p.Key == "otherIdentifier").Value = "updated-otherIdentifier";
        var employeeUpdateEntity = await _authorizationRegistry.UpdateEmployee(employee);
        Assert.NotNull(employeeUpdateEntity);

        allowed = await _authorizationRegistry.Enforce("updated-email@domain.com", "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("updated-1234567890", "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("updated-otherIdentifier", "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce("email@domain.com", "resource", "action");
        Assert.False(allowed);

        allowed = await _authorizationRegistry.Enforce("1234567890", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EmployeeEmailEqualsId()
    {
        var organization = TestData.CreateNewOrganization(nameof(EmployeeEmailEqualsId), 1);
        var employee = TestData.CreateNewEmployee(nameof(EmployeeEmailEqualsId), 1);
        employee.Email = employee.EmployeeId;
        organization.Employees.Add(employee);
        var organizationEntity = await _authorizationRegistry.CreateOrganization(organization);
        Assert.NotNull(organizationEntity);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organizationEntity.Identifier;
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce(organizationEntity.Identifier, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.EmployeeId, "resource", "action");
        Assert.True(allowed);

        allowed = await _authorizationRegistry.Enforce(employee.Email, "resource", "action");
        Assert.True(allowed);
    }
}
