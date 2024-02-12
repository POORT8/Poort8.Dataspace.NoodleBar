using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Components.Templates;

public partial class ORiShareTemplate
{
    [Parameter] public List<Organization>? Organizations { get; set; }
}