using FluentAssertions;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class UseCaseToAuthModelTests
{
    [Fact]
    public void ToPolicyValuesDefault()
    {
        var policy = new Policy("issuerId", "subjectId", "resourceId", "Action");

        var policyValues = policy.ToPolicyValues();

        policyValues[0].Should().BeEquivalentTo(policy.PolicyId);
        policyValues[1].Should().BeEquivalentTo(policy.UseCase.ToLower());
        policyValues[2].Should().BeEquivalentTo(policy.IssuedAt.ToString());
        policyValues[3].Should().BeEquivalentTo(policy.NotBefore.ToString());
        policyValues[4].Should().BeEquivalentTo(policy.Expiration.ToString());
        policyValues[5].Should().BeEquivalentTo(policy.IssuerId.ToLower());
        policyValues[6].Should().BeEquivalentTo(policy.SubjectId.ToLower());
        policyValues[7].Should().BeEquivalentTo(policy.ResourceId.ToLower());
        policyValues[8].Should().BeEquivalentTo(policy.Action.ToLower());
    }

    [Fact]
    public void ToPolicyValuesIshare()
    {
        var policy = new Policy("Ishare", "issuerId", "subjectId", "serviceProvider", "Action", "resourceId", "Type", "Attribute", "license");

        var policyValues = policy.ToPolicyValues();

        policyValues[0].Should().BeEquivalentTo(policy.PolicyId);
        policyValues[1].Should().BeEquivalentTo(policy.UseCase.ToLower());
        policyValues[2].Should().BeEquivalentTo(policy.IssuedAt.ToString());
        policyValues[3].Should().BeEquivalentTo(policy.NotBefore.ToString());
        policyValues[4].Should().BeEquivalentTo(policy.Expiration.ToString());
        policyValues[5].Should().BeEquivalentTo(policy.IssuerId.ToLower());
        policyValues[6].Should().BeEquivalentTo(policy.SubjectId.ToLower());
        policyValues[7].Should().BeEquivalentTo(policy.ServiceProvider!.ToLower());
        policyValues[8].Should().BeEquivalentTo(policy.Action.ToLower());
        policyValues[9].Should().BeEquivalentTo(policy.ResourceId.ToLower());
        policyValues[10].Should().BeEquivalentTo(policy.Type!.ToLower());
        policyValues[11].Should().BeEquivalentTo(policy.Attribute!.ToLower());
        policyValues[12].Should().BeEquivalentTo(policy.License);
    }
}
