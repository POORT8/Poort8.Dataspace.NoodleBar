using Casbin;
using Microsoft.EntityFrameworkCore;
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

    public EnforceTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAuthorizationRegistrySqlite(options => options.ConnectionString = $"Data Source={Guid.NewGuid()}.db");

        serviceCollection.Replace(new ServiceDescriptor(typeof(IRepository), typeof(FakeRepository), ServiceLifetime.Singleton));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _authorizationRegistry = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();

        var factory = _serviceProvider.GetRequiredService<IDbContextFactory<AuthorizationContext>>();
        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }

    [Fact]
    public async Task TestModel()
    {
        IEnforcer? enforcer = _authorizationRegistry
            .GetType()
            .GetField("_enforcer", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(_authorizationRegistry) as IEnforcer;
        Assert.NotNull(enforcer);

        var success = await enforcer.AddNamedGroupingPolicyAsync("subjectGroup", "emp", "org");
        Assert.True(success);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = "org";
        var policyValuesMethod = policy.GetType().GetMethod("ToPolicyValues", BindingFlags.NonPublic | BindingFlags.Instance);
        var policyValues = policyValuesMethod!.Invoke(policy, null) as string[];
        success = await enforcer.AddPolicyAsync(policyValues);
        Assert.True(success);

        var enforcerPolicy = enforcer.GetPolicy();
        Assert.Single(enforcerPolicy);
        Assert.Equal(policyValues, enforcerPolicy.First());

        var enforcerGroup = enforcer.GetNamedGroupingPolicy("subjectGroup");
        Assert.Single(enforcerGroup);
        Assert.Equal(new string[] { "emp", "org" }, enforcerGroup.First());

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
}
