using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Audit;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.AuditLogs;

public partial class Index
{
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }

    public IReadOnlyList<EntityAuditRecord>? EntityAuditRecords;
    public IReadOnlyList<EnforceAuditRecord>? EnforceAuditRecords;

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;
        EntityAuditRecords = (await AuthorizationRegistry!.GetEntityAuditRecords()).OrderByDescending(o => o.Timestamp).Take(25).ToList();
        EnforceAuditRecords = (await AuthorizationRegistry!.GetEnforceAuditRecords()).OrderByDescending(o => o.Timestamp).Take(25).ToList();
    }
}
