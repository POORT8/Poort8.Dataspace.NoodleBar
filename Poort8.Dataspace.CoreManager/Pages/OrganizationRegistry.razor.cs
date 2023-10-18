using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Pages;

public partial class OrganizationRegistry
{
    [Inject] private IOrganizationRegistry? OrganizationRegistryService { get; set; }
    [Inject] private ILogger<OrganizationRegistry>? Logger { get; set; }
    private List<Organization>? _organizations = new();
    private Organization? _selectedOrganization;
    private Organization? _newOrganization;
    private Organization? EditedOrganization => _selectedOrganization ?? _newOrganization;
    private string _role = string.Empty;
    private string _key = string.Empty;
    private string _value = string.Empty;
    private bool _isIdentifier = false;
    private static bool DisableUpdate(Organization organization) => string.IsNullOrWhiteSpace(organization.Name);
    private bool DisableCreate(Organization organization) => DisableUpdate(organization) || string.IsNullOrWhiteSpace(organization.Identifier) || _organizations?.Any(o => organization.Identifier.Equals(o.Identifier, StringComparison.OrdinalIgnoreCase)) == true;

    protected override async Task OnInitializedAsync()
    {
        _organizations = (await OrganizationRegistryService!.ReadOrganizations()).ToList();

        _ = base.OnInitializedAsync();
    }

    private async Task Create()
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Create button clicked for organization {identifier} ({name})", _newOrganization!.Identifier, _newOrganization.Name);
        var organization = await OrganizationRegistryService!.CreateOrganization(_newOrganization!);
        _organizations?.Add(organization);
        _newOrganization = null;
        StateHasChanged();
    }

    private async Task Delete()
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Delete button clicked for organization {identifier} ({name})", _selectedOrganization!.Identifier, _selectedOrganization.Name);
        await OrganizationRegistryService!.DeleteOrganization(_selectedOrganization!.Identifier);
        _organizations?.RemoveAll(p => p.Identifier.Equals(_selectedOrganization!.Identifier, StringComparison.OrdinalIgnoreCase));
        _selectedOrganization = null;
        StateHasChanged();
    }

    private async Task Update()
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Update button clicked for organization {identifier} ({name})", _selectedOrganization!.Identifier, _selectedOrganization.Name);
        var party = await OrganizationRegistryService!.UpdateOrganization(_selectedOrganization!);
        _organizations?.RemoveAll(p => p.Identifier.Equals(party.Identifier, StringComparison.OrdinalIgnoreCase));
        _organizations?.Add(party);
        _selectedOrganization = null;
        StateHasChanged();
    }

    private void AddNew()
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - AddNew button clicked");
        _newOrganization = new Organization("id", "name");
        _selectedOrganization = null;
        StateHasChanged();
    }

    private void OnRowClick(Organization organization)
    {
        Logger?.LogInformation("P8.inf - OrganizationRegistry - Clicked on row with organization {identifier} ({name})", organization!.Identifier, organization.Name);
        _selectedOrganization = organization.DeepCopy();
        _newOrganization = null;
    }

    private void AddRole()
    {
        EditedOrganization!.Roles.Add(new OrganizationRole(_role));
        _role = string.Empty;
    }

    private void DeleteRole()
    {
        EditedOrganization!.Roles.Remove(EditedOrganization.Roles.First(r => r.Role.Equals(_role, StringComparison.OrdinalIgnoreCase)));
        _role = string.Empty;
    }

    private void AddProperty()
    {
        EditedOrganization!.Properties.Add(new Property(_key, _value, _isIdentifier));
        _key = string.Empty;
        _value = string.Empty;
    }

    private void DeleteProperty()
    {
        EditedOrganization!.Properties.Remove(EditedOrganization.Properties.First(p => p.Key.Equals(_key, StringComparison.OrdinalIgnoreCase)));
        _key = string.Empty;
        _value = string.Empty;
    }
}