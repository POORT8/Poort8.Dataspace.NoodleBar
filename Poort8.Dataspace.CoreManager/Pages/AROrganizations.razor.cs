using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Pages;

public partial class AROrganizations
{
    [Inject] private IAuthorizationRegistry? AuthorizationRegistryService { get; set; }
    [Inject] private ILogger<AROrganizations>? Logger { get; set; }
    private List<Organization>? _organizations = new();
    private Organization? _selectedOrganization;
    private Organization? _newOrganization;
    private Organization? EditedOrganization => _selectedOrganization ?? _newOrganization;
    private Employee _employee = new();
    private Property _organizationProperty = new(string.Empty, string.Empty);
    private static bool DisableUpdateOrganization(Organization organization) => string.IsNullOrWhiteSpace(organization.Name);
    private bool DisableCreateOrganization(Organization organization) => DisableUpdateOrganization(organization) || string.IsNullOrWhiteSpace(organization.Identifier) || _organizations?.Any(o => organization.Identifier.Equals(o.Identifier, StringComparison.OrdinalIgnoreCase)) == true;

    protected override async Task OnInitializedAsync()
    {
        _organizations = (await AuthorizationRegistryService!.ReadOrganizations()).ToList();

        _ = base.OnInitializedAsync();
    }

    private void OnOrganizationRowClick(Organization organization)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Clicked on row with organization {identifier} ({name})", organization!.Identifier, organization.Name);
        _selectedOrganization = organization.DeepCopy();
        _newOrganization = null;
        ResetEmployee();
    }

    private void AddNewOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - AddNewOrganization button clicked");
        _newOrganization = new Organization(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        _selectedOrganization = null;
        ResetEmployee();
        StateHasChanged();
    }

    private async Task CreateOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Create button clicked for organization {identifier} ({name})", _newOrganization!.Identifier, _newOrganization.Name);
        var organization = await AuthorizationRegistryService!.CreateOrganization(_newOrganization!);
        _organizations?.Add(organization);
        _newOrganization = null;
        StateHasChanged();
    }

    private async Task DeleteOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Delete button clicked for organization {identifier} ({name})", _selectedOrganization!.Identifier, _selectedOrganization.Name);
        await AuthorizationRegistryService!.DeleteOrganization(_selectedOrganization!.Identifier);
        _organizations?.RemoveAll(p => p.Identifier.Equals(_selectedOrganization!.Identifier, StringComparison.OrdinalIgnoreCase));
        _selectedOrganization = null;
        StateHasChanged();
    }

    private async Task UpdateOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Update button clicked for organization {identifier} ({name})", _selectedOrganization!.Identifier, _selectedOrganization.Name);
        var party = await AuthorizationRegistryService!.UpdateOrganization(_selectedOrganization!);
        _organizations?.RemoveAll(p => p.Identifier.Equals(party.Identifier, StringComparison.OrdinalIgnoreCase));
        _organizations?.Add(party);
        _selectedOrganization = null;
        StateHasChanged();
    }

    private void ResetEmployee()
    {
        _employee = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
            OrganizationId = EditedOrganization!.Identifier,
            Organization = EditedOrganization
        };
    }

    private void AddEmployee()
    {
        EditedOrganization!.Employees.Add(_employee);
        ResetEmployee();
    }

    private void DeleteEmployee()
    {
        EditedOrganization!.Employees.Remove(EditedOrganization.Employees.First(e => e.EmployeeId.Equals(_employee.EmployeeId, StringComparison.OrdinalIgnoreCase)));
        ResetEmployee();
    }

    private void ResetOrganizationProperty()
    {
        _organizationProperty = new(string.Empty, string.Empty);
    }

    private void AddOrganizationProperty()
    {
        EditedOrganization!.Properties.Add(_organizationProperty);
        ResetOrganizationProperty();
    }

    private void DeleteOrganizationProperty()
    {
        EditedOrganization!.Properties.Remove(EditedOrganization.Properties.First(p => p.Key.Equals(_organizationProperty.Key, StringComparison.OrdinalIgnoreCase)));
        ResetOrganizationProperty();
    }
}