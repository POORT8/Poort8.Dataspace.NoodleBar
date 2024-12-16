using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class UseCases
{
    public static readonly Dictionary<string, string> AuthorizationModels = new()
    {
        { "hwct", "default" },
        { "ishare", "ishare" },
        { "isharerules", "isharerules" },
        { "gir", "isharerules" },
        { "noodlebarscopes", "default"},
        { "dvu-mock", "ishare" },
        { "efti", "ishare" }
    };

    public static string GetAuthorizationModel(string useCase)
    {
        return AuthorizationModels.TryGetValue(useCase.ToLower(), out var authModel) ? authModel : "default";
    }

    public static string GetPolicyType(Policy policy)
    {
        var useCase = GetAuthorizationModel(policy.UseCase);

        if (useCase == "default")
        {
            return "p";
        }
        else
        {
            return "p" + GetAuthorizationModel(policy.UseCase).ToLower();
        }
    }
}
