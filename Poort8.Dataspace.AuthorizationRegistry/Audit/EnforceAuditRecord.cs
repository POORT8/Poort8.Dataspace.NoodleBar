using Poort8.Dataspace.AuthorizationRegistry.Entities;
using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Audit;
public class EnforceAuditRecord
{
    [Key]
    public string Id { get; init; }
    public DateTime Timestamp { get; set; }
    public string User { get; set; }
    public string UseCase { get; set; }
    public string SubjectId { get; set; }
    public string ResourceId { get; set; }
    public string Action { get; set; }
    public bool Allow { get; set; }
    public string Explain { get; set; }
    public string? IssuerId { get; set; }
    public string? ServiceProvider { get; set; }
    public string? Type { get; set; }
    public string? Attribute { get; set; }
    public string? RequestContext { get; set; }

    public EnforceAuditRecord(string id, DateTime timestamp, string user, string useCase, string subjectId, string resourceId, string action, bool allow, string explain)
    {
        Id = id;
        Timestamp = timestamp;
        User = user;
        UseCase = useCase;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
        Allow = allow;
        Explain = explain;
    }

    public EnforceAuditRecord(string user, string useCase, string subjectId, string resourceId, string action, bool allow, List<Policy>? explains = null)
    {
        Id = $"{user}-{useCase}-{Guid.NewGuid()}";
        Timestamp = DateTime.Now;
        User = user;
        UseCase = useCase;
        SubjectId = subjectId;
        ResourceId = resourceId;
        Action = action;
        Allow = allow;
        Explain = explains?.Count > 0 ? string.Join(", ", explains.Select(p => p.PolicyId)) : "";
    }
}