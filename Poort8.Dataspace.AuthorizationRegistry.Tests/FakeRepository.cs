using Force.DeepCloner;
using Policy = Poort8.Dataspace.AuthorizationRegistry.Entities.Policy;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;

public class FakeRepository : IRepository
{
    private readonly List<Policy> _policies = new();

    public Task<Policy> Create(Policy policy)
    {
        if (policy.Action == "exception") throw new Exception("exception");

        _policies.Add(policy);
        return Task.FromResult(policy.DeepClone());
    }

    public Task<Policy?> Read(string policyId)
    {
        return Task.FromResult(_policies.FirstOrDefault(p => p.PolicyId == policyId));
    }

    public async Task<Policy> Update(Policy policy)
    {
        _policies.RemoveAll(p => p.PolicyId == policy.PolicyId);
        var updatedPolicy = await Create(policy);
        return updatedPolicy;
    }

    public Task<bool> Delete(string policyId)
    {
        _policies.RemoveAll(p => p.PolicyId == policyId);
        return Task.FromResult(true);
    }
}