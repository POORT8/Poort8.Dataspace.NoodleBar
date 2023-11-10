using Casbin.Model;

namespace Poort8.Dataspace.AuthorizationRegistry;
internal class EnforcerModel
{
    internal static IModel Create()
    {
        //TODO: Refactor?
        var model = DefaultModel.Create();

        model.AddDef("r", "r", "useCase, now, subjectId, resourceId, action");
        model.AddDef("p", "p", "policyId, useCase, issuedAt, notBefore, expiration, issuerId, subjectId, resourceId, action");
        model.AddDef("g", "subjectGroup", "_, _");
        model.AddDef("g", "resourceGroup", "_, _");
        model.AddDef("e", "e", "some(where (p.eft == allow))");
        model.AddDef("m", "m", "r.useCase == p.useCase && subjectGroup(r.subjectId, p.subjectId) && resourceGroup(r.resourceId, p.resourceId) && r.action == p.action && r.now >= p.notBefore && r.now <= p.expiration");

        return model;
    }
}
