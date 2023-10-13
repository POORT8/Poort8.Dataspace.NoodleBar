using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.OrganizationRegistry;
public class AuditRecord
{
    [Key]
    public string Id { get; init; }
    public DateTime Timestamp { get; set; }
    public string Caller { get; set; }
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string Action { get; set; }
    public string Entity { get; set; }

    public AuditRecord(DateTime timestamp, string caller, string entityType, string entityId, string action, string entity)
    {
        var guid = Guid.NewGuid().ToString().Split('-');
        Id = $"{entityType}-{entityId}-{guid[0]}";
        Timestamp = timestamp;
        Caller = caller;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Entity = entity;
    }
}