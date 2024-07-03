using Poort8.Ishare.Core.Models;
using static Poort8.Ishare.Core.Models.DelegationMask;

namespace Poort8.Dataspace.API.Tests;

public class DelegationEvidenceCreatorTests
{
    [Fact]
    public void CreateShouldReturnDelegationEvidenceWithPermit()
    {
        var delegationMask = CreateDelegationMask("Permit");
        var explainPolicy = new AuthorizationRegistry.Entities.Policy("policyIssuer", "accessSubject", "identifier", "action");

        var delegationEvidence = DelegationEvidenceCreator.Create(true, delegationMask, explainPolicy);

        Assert.NotNull(delegationEvidence);
        Assert.Equal("Permit", delegationEvidence.PolicySets[0].Policies[0].Rules[0].Effect);
        Assert.Equal("policyIssuer", delegationEvidence.PolicyIssuer);
        Assert.Equal("accessSubject", delegationEvidence.Target.AccessSubject);
        Assert.Equal("identifier", delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers[0]);
        Assert.Equal("action", delegationEvidence.PolicySets[0].Policies[0].Target.Actions[0]);
        Assert.Equal(0, delegationEvidence.PolicySets[0].MaxDelegationDepth);
        Assert.NotEqual(-1, delegationEvidence.NotBefore);
        Assert.NotEqual(-1, delegationEvidence.NotOnOrAfter);
    }

    [Fact]
    public void CreateShouldReturnDelegationEvidenceWithDeny()
    {
        var delegationMask = CreateDelegationMask("Deny");
        var explainPolicy = new AuthorizationRegistry.Entities.Policy("policyIssuer", "accessSubject", "identifier", "action");

        var delegationEvidence = DelegationEvidenceCreator.Create(false, delegationMask, explainPolicy);

        Assert.NotNull(delegationEvidence);
        Assert.Equal("Deny", delegationEvidence.PolicySets[0].Policies[0].Rules[0].Effect);
        Assert.Equal("policyIssuer", delegationEvidence.PolicyIssuer);
        Assert.Equal("accessSubject", delegationEvidence.Target.AccessSubject);
        Assert.Equal("identifier", delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers[0]);
        Assert.Equal("action", delegationEvidence.PolicySets[0].Policies[0].Target.Actions[0]);
        Assert.NotEqual(-1, delegationEvidence.NotBefore);
        Assert.NotEqual(-1, delegationEvidence.NotOnOrAfter);
    }

    [Fact]
    public void CreateShouldReturnDelegationEvidenceWithDenyNoExpain()
    {
        var delegationMask = CreateDelegationMask("Deny");

        var delegationEvidence = DelegationEvidenceCreator.Create(false, delegationMask, null);

        Assert.NotNull(delegationEvidence);
        Assert.Equal("Deny", delegationEvidence.PolicySets[0].Policies[0].Rules[0].Effect);
        Assert.Equal("policyIssuer", delegationEvidence.PolicyIssuer);
        Assert.Equal("accessSubject", delegationEvidence.Target.AccessSubject);
        Assert.Equal("identifier", delegationEvidence.PolicySets[0].Policies[0].Target.Resource.Identifiers[0]);
        Assert.Equal("action", delegationEvidence.PolicySets[0].Policies[0].Target.Actions[0]);
        Assert.Equal(-1, delegationEvidence.NotBefore);
        Assert.Equal(-1, delegationEvidence.NotOnOrAfter);
    }

    private static DelegationMask CreateDelegationMask(string effect)
    {
        var delegationRequest = new DelegationRequestObject
        {
            PolicyIssuer = "policyIssuer",
            Target = new DelegationRequestObject.TargetObject
            {
                AccessSubject = "accessSubject"
            },
            PolicySets =
            [
                new DelegationRequestObject.PolicySet
                {
                    Policies =
                    [
                        new DelegationRequestObject.PolicySet.Policy
                        {
                            Target = new DelegationRequestObject.PolicySet.Policy.TargetObject
                            {
                                Resource = new DelegationRequestObject.PolicySet.Policy.TargetObject.ResourceObject
                                {
                                    Type = "resourceType",
                                    Identifiers = ["identifier"],
                                    Attributes = ["attribute"]
                                },
                                Actions = ["action"],
                                Environment = new DelegationRequestObject.PolicySet.Policy.TargetObject.EnvironmentObject
                                {
                                    ServiceProviders = ["serviceProvider"]
                                }
                            },
                            Rules =
                            [
                                new DelegationRequestObject.PolicySet.Policy.Rule
                                {
                                    Effect = effect
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        return new DelegationMask
        {
            DelegationRequest = delegationRequest
        };
    }
}