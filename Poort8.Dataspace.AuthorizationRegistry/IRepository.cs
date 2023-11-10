using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public interface IRepository
{
    Task<Policy> Create(Policy policy);
    Task<Policy?> Read(string policyId);
    Task<Policy> Update(Policy policy);
    Task<bool> Delete(string policyId);
}