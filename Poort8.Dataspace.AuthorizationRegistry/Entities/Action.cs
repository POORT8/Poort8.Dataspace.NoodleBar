using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Action //https://schema.org/Action
{
    [Key]
    public string ActionId { get; set; }
    public string Name { get; set; }
    public string AdditionalType { get; set; }

    public Action(string actionId, string name, string additionalType)
    {
        ActionId = actionId;
        Name = name;
        AdditionalType = additionalType;
    }
}
