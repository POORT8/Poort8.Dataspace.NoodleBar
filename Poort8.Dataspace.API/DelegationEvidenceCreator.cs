using Poort8.Ishare.Core.Models;

namespace Poort8.Dataspace.API;

public class DelegationEvidenceCreator
{
    public static DelegationEvidence Create(bool allowed, DelegationMask delegationMask, AuthorizationRegistry.Entities.Policy? explainPolicy)
    {
        var resourceTarget = new ResourceTarget(
        new Resource(
                delegationMask.DelegationRequest.PolicySets[0].Policies[0].Target.Resource.Type ?? "",
                [delegationMask.DelegationRequest.PolicySets[0].Policies[0].Target.Resource.Identifiers[0]],
                [delegationMask.DelegationRequest.PolicySets[0].Policies[0].Target.Resource.Attributes[0] ?? ""]),
                new ServiceProviderEnvironment([delegationMask.DelegationRequest.PolicySets[0].Policies[0].Target.Environment.ServiceProviders[0] ?? ""]),
                [delegationMask.DelegationRequest.PolicySets[0].Policies[0].Target.Actions[0]]);

        var policy = new Policy(
            resourceTarget,
            [new Rule(allowed ? "Permit" : "Deny")]);

        var policySet = new PolicySet(
            0,
            new LicenseTarget(new LicenseEnvironment([explainPolicy?.License ?? "Not set"])),
            [policy]);

        var delegationEvidence = new DelegationEvidence(
            explainPolicy?.NotBefore ?? -1,
            explainPolicy?.Expiration ?? -1,
            delegationMask.DelegationRequest.PolicyIssuer,
            new AccessSubjectTarget(delegationMask.DelegationRequest.Target.AccessSubject),
            [policySet]);

        return delegationEvidence;
    }
}