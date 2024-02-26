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

    public ICollection<PolicyProperty> Properties { get; set; } = new List<PolicyProperty>();

    public Policy(string issuerId, string subjectId, string resourceId, string action)
    {
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
    }

    public Policy(string issuerId, string subjectId, string resourceId, string action, ICollection<PolicyProperty> properties)
    {
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
        Properties = properties;
    }

    public Policy(string useCase, string issuerId, string subjectId, string resourceId, string action)
    {
        UseCase = useCase;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
    }

    public Policy(string useCase, string issuerId, string subjectId, string resourceId, string action, ICollection<PolicyProperty> properties)
    {
        UseCase = useCase;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
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

    internal string[] ToPolicyValues()
    {
        return new string[]
        {
            PolicyId,
            UseCase,
            IssuedAt.ToString(),
            NotBefore.ToString(),
            Expiration.ToString(),
            IssuerId,
            SubjectId,
            ResourceId,
            Action
        };
    }
}