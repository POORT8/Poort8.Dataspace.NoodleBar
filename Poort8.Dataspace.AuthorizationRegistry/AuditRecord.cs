using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry;
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

    public AuditRecord(DateTime timestamp, string user, string entityType, string entityId, string action, string entity)
    {
        var guid = Guid.NewGuid().ToString().Split('-');
        Id = $"{entityType}-{entityId}-{guid[0]}";
        Timestamp = timestamp;
        User = user;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Entity = entity;
    }
}