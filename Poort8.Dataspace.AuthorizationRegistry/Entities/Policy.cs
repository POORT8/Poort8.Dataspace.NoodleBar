using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Policy
{
    [Key]
    public string PolicyId { get; init; } = Guid.NewGuid().ToString();
    public string UseCase { get; set; } = "default";
    public long IssuedAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    public long NotBefore { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    public long Expiration { get; set; } = DateTimeOffset.Now.AddYears(1).ToUnixTimeSeconds();
    public string IssuerId { get; set; }
    public string SubjectId { get; set; }
    public string? ServiceProvider { get; set; }
    public string Action { get; set; }
    public string ResourceId { get; set; }
    public string? Type { get; set; }
    public string? Attribute { get; set; }
    public string? License { get; set; }
    public string? Rules { get; set; }

    public ICollection<PolicyProperty> Properties { get; set; } = new List<PolicyProperty>();

    public Policy(
        string issuerId,
        string subjectId,
        string resourceId,
        string action)
    {
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
    }

    public Policy(
        string issuerId,
        string subjectId,
        string resourceId,
        string action,
        ICollection<PolicyProperty> properties)
    {
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
        Properties = properties;
    }

    public Policy(
        string useCase,
        string issuerId,
        string subjectId,
        string resourceId,
        string action)
    {
        UseCase = useCase;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
    }

    public Policy(
        string useCase,
        string issuerId,
        string subjectId,
        string resourceId,
        string action,
        ICollection<PolicyProperty> properties)
    {
        UseCase = useCase;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
        Properties = properties;
    }

    public Policy(
        string useCase,
        string issuerId,
        string subjectId,
        string? serviceProvider,
        string action,
        string resourceId,
        string? type,
        string? attribute,
        string? license)
    {
        UseCase = useCase;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ServiceProvider = serviceProvider;
        Action = action;
        ResourceId = resourceId;
        Type = type;
        Attribute = attribute;
        License = license;
    }

    public Policy(
        string useCase,
        string issuerId,
        string subjectId,
        string? serviceProvider,
        string action,
        string resourceId,
        string? type,
        string? attribute,
        string? license,
        ICollection<PolicyProperty> properties) : this(useCase, issuerId, subjectId, serviceProvider, action, resourceId, type, attribute, license)
    {
        Properties = properties;
    }

    public Policy(
        string useCase,
        long issuedAt,
        long notBefore,
        long expiration,
        string issuerId,
        string subjectId,
        string? serviceProvider,
        string action,
        string resourceId,
        string? type,
        string? attribute,
        string? license,
        string? rules)
    {
        UseCase = useCase;
        IssuedAt = issuedAt;
        NotBefore = notBefore;
        Expiration = expiration;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ServiceProvider = serviceProvider;
        Action = action;
        ResourceId = resourceId;
        Type = type;
        Attribute = attribute;
        License = license;
        Rules = rules;
    }

    public Policy(
        string useCase,
        long issuedAt,
        long notBefore,
        long expiration,
        string issuerId,
        string subjectId,
        string? serviceProvider,
        string action,
        string resourceId,
        string? type,
        string? attribute,
        string? license,
        string? rules,
        ICollection<PolicyProperty> properties) : this(useCase, issuedAt, notBefore, expiration, issuerId, subjectId, serviceProvider, action, resourceId, type, attribute, license, rules)
    {
        Properties = properties;
    }

    public class PolicyProperty
    {
        [Key]
        public string PropertyId { get; init; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public PolicyProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public PolicyProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }

    public string[] ToPolicyValues()
    {
        var authorizationModel = UseCases.GetAuthorizationModel(UseCase);

        return authorizationModel switch
        {
            "default" => [
                PolicyId,
                UseCase.ToLower(),
                IssuedAt.ToString(),
                NotBefore.ToString(),
                Expiration.ToString(),
                IssuerId.ToLower(),
                SubjectId.ToLower(),
                ResourceId.ToLower(),
                Action.ToLower()],
            "ishare" => [
                PolicyId,
                UseCase.ToLower(),
                IssuedAt.ToString(),
                NotBefore.ToString(),
                Expiration.ToString(),
                IssuerId.ToLower(),
                SubjectId.ToLower(),
                string.IsNullOrEmpty(ServiceProvider) ? "*" : ServiceProvider.ToLower(),
                string.IsNullOrEmpty(Action) ? "*" : Action.ToLower(),
                string.IsNullOrEmpty(ResourceId) ? "*" : ResourceId.ToLower(),
                string.IsNullOrEmpty(Type) ? "*" : Type.ToLower(),
                string.IsNullOrEmpty(Attribute) ? "*" : Attribute.ToLower(),
                License ?? "Not set"],
            "isharerules" => [
                PolicyId,
                UseCase.ToLower(),
                IssuedAt.ToString(),
                NotBefore.ToString(),
                Expiration.ToString(),
                IssuerId.ToLower(),
                SubjectId.ToLower(),
                string.IsNullOrEmpty(ServiceProvider) ? "*" : ServiceProvider.ToLower(),
                string.IsNullOrEmpty(Action) ? "*" : Action.ToLower(),
                string.IsNullOrEmpty(ResourceId) ? "*" : ResourceId.ToLower(),
                string.IsNullOrEmpty(Type) ? "*" : Type.ToLower(),
                string.IsNullOrEmpty(Attribute) ? "*" : Attribute.ToLower(),
                License ?? "Not set",
                Rules ?? string.Empty],
            _ => [
                PolicyId,
                UseCase.ToLower(),
                IssuedAt.ToString(),
                NotBefore.ToString(),
                Expiration.ToString(),
                IssuerId.ToLower(),
                SubjectId.ToLower(),
                ResourceId.ToLower(),
                Action.ToLower()],
        };
    }
}
