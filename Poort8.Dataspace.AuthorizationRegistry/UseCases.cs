using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.AuthorizationRegistry;
public class UseCases
{
    private static readonly Dictionary<string, string> _authorizationModels = new()
    {
        { "hwct", "default" },
        { "ishare", "ishare" },
        { "isharerules", "isharerules" },
        { "gir", "isharerules" },
        { "noodlebarscopes", "default"}
    };

    public static string GetAuthorizationModel(string useCase)
    {
        return _authorizationModels.TryGetValue(useCase.ToLower(), out var authModel) ? authModel : "default";
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
