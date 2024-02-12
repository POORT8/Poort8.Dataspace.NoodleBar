using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Pages;

public partial class OrganizationRegistry
{
    [Inject] private IConfiguration? Config { get; set; }
    [Inject] private IOrganizationRegistry? OrganizationRegistryService { get; set; }
    [Inject] private ILogger<OrganizationRegistry>? Logger { get; set; }
    private List<Organization>? _organizations = new();

    protected override async Task OnInitializedAsync()
    {
        _organizations = (await OrganizationRegistryService!.ReadOrganizations()).ToList();
        _ = base.OnInitializedAsync();
    }

    private async Task CreateOrganization(Organization organization)
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Create event called for organization {identifier} ({name})", organization.Identifier, organization.Name);
        organization = await OrganizationRegistryService!.CreateOrganization(organization);
        _organizations?.Add(organization);
        StateHasChanged();
    }

    private async Task DeleteOrganization(Organization organization)
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Delete event called for organization {identifier} ({name})", organization.Identifier, organization.Name);
        await OrganizationRegistryService!.DeleteOrganization(organization.Identifier);
        _organizations?.Remove(organization);
        StateHasChanged();
    }

    private async Task UpdateOrganization(Organization organization)
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Update event called for organization {identifier} ({name})", organization.Identifier, organization.Name);
        var party = await OrganizationRegistryService!.UpdateOrganization(organization);
        _organizations?.RemoveAll(p => p.Identifier.Equals(party.Identifier, StringComparison.OrdinalIgnoreCase));
        _organizations?.Add(party);
        StateHasChanged();
    }
}