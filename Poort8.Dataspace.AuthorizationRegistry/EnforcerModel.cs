using Casbin.Model;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class EnforcerModel
{
    public static IModel Create()
    {
        //TODO: Refactor? Using magic strings in places.
        var model = DefaultModel.Create();

        //generic
        model.AddDef("g", "subjectGroup", "_, _, _");
        model.AddDef("g", "resourceGroup", "_, _, _");

        model.AddDef("e", "e", "some(where (p.eft == allow))");

        AddDefaultModel(model);
        AddIshareModel(model);
        AddIshareRulesModel(model);

        return model;
    }

    private static void AddDefaultModel(IModel model)
    {
        model.AddDef("r", "r", "useCase, now, subjectId, resourceId, action");
        model.AddDef("p", "p", "policyId, useCase, issuedAt, notBefore, expiration, issuerId, subjectId, resourceId, action");

        var modelDefinition = """"
        r.useCase == p.useCase && 
        subjectGroup(r.subjectId, p.subjectId, r.useCase) && 
        resourceGroup(r.resourceId, p.resourceId, r.useCase) && 
        r.action == p.action && 
        r.now >= p.notBefore && 
        r.now <= p.expiration
        """";
        model.AddDef("m", "m", modelDefinition.Replace("\n", "").Replace("\r", "").Trim());
    }

    private static void AddIshareModel(IModel model)
    {
        model.AddDef("r", "rishare", "useCase, now, issuerId, subjectId, serviceProvider, action, resourceId, type, attribute");
        model.AddDef("p", "pishare", "policyId, useCase, issuedAt, notBefore, expiration, issuerId, subjectId, serviceProvider, action, resourceId, type, attribute, license");

        var modelDefinition = """"
        rishare.useCase == pishare.useCase && 
        rishare.issuerId == pishare.issuerId && 
        (pishare.serviceProvider == "*" || rishare.serviceProvider == pishare.serviceProvider) && 
        subjectGroup(rishare.subjectId, pishare.subjectId, rishare.useCase) && 
        (pishare.resourceId == "*" || resourceGroup(rishare.resourceId, pishare.resourceId, rishare.useCase)) && 
        (pishare.action == "*" || rishare.action == pishare.action) && 
        (pishare.type == "*" || rishare.type == pishare.type) && 
        (pishare.attribute == "*" || rishare.attribute == pishare.attribute) && 
        rishare.now >= pishare.notBefore && 
        rishare.now <= pishare.expiration
        """";
        model.AddDef("m", "mishare", modelDefinition.Replace("\n", "").Replace("\r", "").Trim());
    }

    private static void AddIshareRulesModel(IModel model)
    {
        model.AddDef("r", "risharerules", "useCase, now, issuerId, subjectId, serviceProvider, action, resourceId, type, attribute, context");
        model.AddDef("p", "pisharerules", "policyId, useCase, issuedAt, notBefore, expiration, issuerId, subjectId, serviceProvider, action, resourceId, type, attribute, license, rules");

        var modelDefinition = """"
        risharerules.useCase == pisharerules.useCase && 
        risharerules.issuerId == pisharerules.issuerId && 
        (pisharerules.serviceProvider == "*" || risharerules.serviceProvider == pisharerules.serviceProvider) && 
        subjectGroup(risharerules.subjectId, pisharerules.subjectId, risharerules.useCase) && 
        (pisharerules.resourceId == "*" || resourceGroup(risharerules.resourceId, pisharerules.resourceId, risharerules.useCase)) && 
        (pisharerules.action == "*" || risharerules.action == pisharerules.action) && 
        (pisharerules.type == "*" || risharerules.type == pisharerules.type) && 
        (pisharerules.attribute == "*" || risharerules.attribute == pisharerules.attribute) && 
        risharerules.now >= pisharerules.notBefore && 
        risharerules.now <= pisharerules.expiration && 
        EnforceIshareRules(pisharerules.rules, risharerules.context)
        """";
        model.AddDef("m", "misharerules", modelDefinition.Replace("\n", "").Replace("\r", "").Trim());
    }
}
