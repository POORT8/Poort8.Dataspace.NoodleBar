using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Extensions;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Fakes;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class RuleTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IAuthorizationRegistry _authorizationRegistry;

    public RuleTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAuthorizationRegistrySqlite(options => { });

        serviceCollection.Replace(new ServiceDescriptor(typeof(IRepository), typeof(FakeRepository), ServiceLifetime.Singleton));

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _authorizationRegistry = _serviceProvider.GetRequiredService<IAuthorizationRegistry>();
    }

    [Fact]
    public async Task ExplainedEnforceIshareNoRules()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "isharerules";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute", "test", useCase: "isharerules");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task ExplainedEnforceIshareRules()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "isharerules";
        policy.Rules = "StringEquals(test);";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute", "test", useCase: "isharerules");
        Assert.True(allowed);
        Assert.Single(explainPolicy);
        Assert.Equal(policyEntity.PolicyId, explainPolicy.First().PolicyId);
    }

    [Fact]
    public async Task ExplainedEnforceIshareRulesFail()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "isharerules";
        policy.Rules = "StringEquals(test);";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute", "not-test", useCase: "isharerules");
        Assert.False(allowed);
        Assert.Empty(explainPolicy);
    }

    [Fact]
    public async Task ExplainedEnforceIshareUnkownRule()
    {
        var policy = TestData.CreateNewPolicy();
        policy.UseCase = "isharerules";
        policy.Rules = "UnknownRule(test);";
        var policyEntity = await _authorizationRegistry.CreatePolicy(policy);
        Assert.NotNull(policyEntity);

        var (allowed, explainPolicy) = await _authorizationRegistry.ExplainedEnforce("issuer", "subject", "serviceProvider", "action", "resource", "type", "attribute", "not-test", useCase: "isharerules");
        Assert.False(allowed);
        Assert.Empty(explainPolicy);
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
}
