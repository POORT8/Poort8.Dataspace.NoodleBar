namespace Poort8.Dataspace.AuthorizationRegistry;
public class RuleEnforcers
{
    public static bool AllowAll(string rules, string requestContext)
    {
        return true;
    }

    public static bool EnforceIshareRules(string rules, string requestContext)
    {
        if (string.IsNullOrEmpty(rules)) return true;

        var (ruleName, parameters) = ParseRule(rules);
        switch (ruleName)
        {
            case "StringEquals":
                return requestContext.Equals(parameters[0], StringComparison.OrdinalIgnoreCase);
            default:
                return false;
        }
    }

    public static bool EnforceGirRules(string rules, string requestContext)
    {
        return true;
    }

    public static Func<string, string, bool> GetRuleEnforcer(string useCase)
    {
        return useCase.ToLower() switch
        {
            "isharerules" => EnforceIshareRules,
            "gir" => EnforceGirRules,
            _ => throw new Exception($"A rules enforcer for use case {useCase} is not set.")
        };
    }

    private static (string ruleName, string[] parameters) ParseRule(string rule)
    {
        var ruleName = rule.Substring(0, rule.IndexOf('('));
        var parameters = rule.Substring(rule.IndexOf('(') + 1, rule.LastIndexOf(')') - rule.IndexOf('(') - 1).Split(',');
        return (ruleName, parameters);
    }
}
