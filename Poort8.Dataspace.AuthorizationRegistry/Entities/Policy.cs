using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Policy
{
    [Key]
    public string PolicyId { get; init; } = Guid.NewGuid().ToString();
    public string UseCase { get; set; } = "default"; //TODO: Check with Casbin implementation
    public long IssuedAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    public long NotBefore { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    public long Expiration { get; set; } = DateTimeOffset.Now.AddYears(1).ToUnixTimeSeconds();
    public string IssuerId { get; set; }
    public string SubjectId { get; set; }
    public string ResourceId { get; set; }
    public string Action { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();

    public Policy(string issuerId, string subjectId, string resourceId, string action)
    {
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
    }

    public Policy(string issuerId, string subjectId, string resourceId, string action, ICollection<Property> properties)
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

    public Policy(string useCase, string issuerId, string subjectId, string resourceId, string action, ICollection<Property> properties)
    {
        UseCase = useCase;
        IssuerId = issuerId;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
        Properties = properties;
    }

    public Policy DeepCopy()
    {
        return new Policy(
            UseCase,
            IssuerId,
            SubjectId,
            ResourceId,
            Action,
            Properties.Select(p => new Property(p.Key, p.Value, p.IsIdentifier)).ToList())
        {
            PolicyId = PolicyId,
            IssuedAt = IssuedAt,
            NotBefore = NotBefore,
            Expiration = Expiration
        };
    }
}