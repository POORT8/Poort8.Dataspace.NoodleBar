using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.Extensions;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Components.Templates;

public partial class ORPoort8Template
{
    [Inject] private ILogger<ORPoort8Template>? Logger { get; set; }
    [Parameter] public List<Organization>? Organizations { get; set; }
    [Parameter] public EventCallback<Organization> CreateOrganizationCallback { get; set; }
    [Parameter] public EventCallback<Organization> UpdateOrganizationCallback { get; set; }
    [Parameter] public EventCallback<Organization> DeleteOrganizationCallback { get; set; }
    private Organization? _selectedOrganization;
    private Organization? _newOrganization;
    private Organization? EditedOrganization => _selectedOrganization ?? _newOrganization;
    private OrganizationRole _role = new(string.Empty);
    private Property _property = new(string.Empty, string.Empty);
    private static bool DisableUpdate(Organization organization) => string.IsNullOrWhiteSpace(organization.Name);
    private bool DisableCreate(Organization organization) => DisableUpdate(organization) || string.IsNullOrWhiteSpace(organization.Identifier) || Organizations?.Any(o => organization.Identifier.Equals(o.Identifier, StringComparison.OrdinalIgnoreCase)) == true;

    private void AddNewOrganization()
    {
        Logger?.LogInformation("P8.inf - ORPoort8Template - AddNew button clicked");
        _newOrganization = new Organization(string.Empty, string.Empty);
        _selectedOrganization = null;
        StateHasChanged();
    }

    private void OnOrganizationRowClick(Organization organization)
    {
        Logger?.LogInformation("P8.inf - ORPoort8Template - Clicked on row with organization {identifier} ({name})", organization.Identifier, organization.Name);
        _selectedOrganization = Organizations?.FirstOrDefault(o => o.Identifier.Equals(organization.Identifier, StringComparison.OrdinalIgnoreCase))?.DeepCopy();
        _newOrganization = null;
    }

    private void AddRole()
    {
        EditedOrganization!.Roles.Add(_role);
        _role = new(string.Empty);
    }

    private void AddProperty()
    {
        EditedOrganization!.Properties.Add(_property);
        _property = new(string.Empty, string.Empty);
    }

    private void CreateOrganization()
    {
        _ = CreateOrganizationCallback.InvokeAsync(_newOrganization!);
        _newOrganization = null;
    }

    private void UpdateOrganization()
    {
        _ = UpdateOrganizationCallback.InvokeAsync(_selectedOrganization!);
        _selectedOrganization = null;
    }

    private void DeleteOrganization(Organization organization)
    {
        _ = DeleteOrganizationCallback.InvokeAsync(organization);
    }
}