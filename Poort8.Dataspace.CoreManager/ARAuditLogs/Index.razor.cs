using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Audit;

namespace Poort8.Dataspace.CoreManager.ARAuditLogs;

public partial class Index : ComponentBase
{
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }

    public IReadOnlyList<EntityAuditRecord>? EntityAuditRecords;
    public IReadOnlyList<EnforceAuditRecord>? EnforceAuditRecords;

    protected override async Task OnInitializedAsync()
    {
        EntityAuditRecords = [.. (await AuthorizationRegistry!.GetEntityAuditRecords()).OrderByDescending(o => o.Timestamp)];
        EnforceAuditRecords = [.. (await AuthorizationRegistry!.GetEnforceAuditRecords()).OrderByDescending(o => o.Timestamp)];
    }
}
