using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using System.Reflection;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class EnforceTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAuthorizationRegistry _authorizationRegistry;
    private static Policy NewPolicy() => new("issuer", "subject", "resource", "action");

    public EnforceTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAuthorizationRegistrySqlite(options => options.ConnectionString = $"Data Source={Guid.NewGuid()}.db");

        serviceCollection.Replace(new ServiceDescriptor(typeof(IRepository), typeof(FakeRepository), ServiceLifetime.Singleton));

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
    public async Task CreateAndEnforcePolicy()
    {
        var policyEntity = await _authorizationRegistry.CreatePolicy(NewPolicy());
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.True(allowed);
    }

    [Fact]
    public async Task CreateDuplicatePolicies()
    {
        var policy = NewPolicy();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        policy.Action = "exception";
        await Assert.ThrowsAsync<Exception>(async () => await _authorizationRegistry.CreatePolicy(policy));
    }

    [Fact]
    public async Task EnforceNotBefore()
    {
        var policy = NewPolicy();
        policy.NotBefore = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EnforceExpiration()
    {
        var policy = NewPolicy();
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
        var policy = NewPolicy();
        policy.Action = "fail";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var allowed = await _authorizationRegistry.Enforce("subject", "resource", "action");
        Assert.False(allowed);
    }

    [Fact]
    public async Task EnforceOtherUseCase()
    {
        var policy = NewPolicy();
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
        var policy = NewPolicy();
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
        var policy = NewPolicy();
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
        var policyEntity = await _authorizationRegistry.CreatePolicy(NewPolicy());
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("subject", "resource", "action");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task ExplainedEnforceOtherUseCase()
    {
        var policy = NewPolicy();
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
}
