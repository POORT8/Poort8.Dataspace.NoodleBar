using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.OrganizationRegistry;
public class AuditRecord
{
    [Key]
    public string Id { get; init; }
    public DateTime Timestamp { get; set; }
    public string User { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string Action { get; set; }
    public string Entity { get; set; }

    public AuditRecord(string user, string entityType, string entityId, string action, string entity)
    {
        Id = $"{entityType}-{entityId}-{Guid.NewGuid()}";
        Timestamp = DateTime.Now;
        User = user;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Entity = entity;
    }
}