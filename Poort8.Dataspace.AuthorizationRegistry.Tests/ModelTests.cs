using Casbin;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class ModelTests
{
    private readonly Enforcer _enforcer;

    public ModelTests()
    {
        _enforcer = new Enforcer(EnforcerModel.Create());
    }

    [Fact]
    public async Task TestModel()
    {
        var success = await _enforcer.AddPolicyAsync("policyId", "default", "0", "1", "3", "issuerId", "subjectId", "resourceId", "action");
        Assert.True(success);

        var allowed = await _enforcer.EnforceAsync("default", "1", "subjectId", "resourceId", "action");
        Assert.True(allowed);
    }

    [Fact]
    public async Task TestSubjectGroup()
    {
        var success = await _enforcer.AddPolicyAsync("policyId", "default", "0", "1", "3", "issuerId", "org", "resourceId", "action");
        Assert.True(success);

        success = await _enforcer.AddNamedGroupingPolicyAsync("subjectGroup", "emp", "org");
        Assert.True(success);

        var allowed = await _enforcer.EnforceAsync("default", "1", "org", "resourceId", "action");
        Assert.True(allowed);
        
        allowed = await _enforcer.EnforceAsync("default", "1", "emp", "resourceId", "action");
        Assert.True(allowed);
    }
}
