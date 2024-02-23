using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.AROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.AROrganizations;

public partial class Details : IDisposable
{
    private bool disposedValue;
    [Parameter]
    public required string Identifier { get; set; }
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }
    public Organization Organization { get; private set; } = default!;

    protected override void OnInitialized()
    {
        StateContainer.OnChange += StateHasChanged;
        Organization = StateContainer.CurrentAROrganization!;
    }

    private async Task EditClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Organization",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<OrganizationDialog>(Organization, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            Organization = await AuthorizationRegistry.UpdateOrganization((Organization)result.Data);
        }
    }

    private void BackClicked()
    {
        StateContainer.CurrentAROrganization = Organization;
        NavigationManager!.NavigateTo($"/ar/organizations");
    }

    private async Task AddNewEmployeeClicked() //TODO: To Employee component?
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Employee",
            PrimaryAction = "Add New Employee",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewEmployeeClicked)
        };

        var employee = new Employee("", "", "", "", "");
        await DialogService.ShowDialogAsync<EmployeeDialog>(employee, parameters);
    }

    private async Task HandleAddNewEmployeeClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.AddNewEmployeeToOrganization(Organization.Identifier, (Employee)result.Data);
            Organization = await AuthorizationRegistry.ReadOrganization(Organization.Identifier) ?? Organization;
        }
    }

    private async Task EmployeeEditClicked(Employee employee)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Employee",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditEmployeeClicked)
        };

        await DialogService.ShowDialogAsync<EmployeeDialog>(employee, parameters);
    }

    private async Task HandleEditEmployeeClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateEmployee((Employee)result.Data);
            Organization = await AuthorizationRegistry.ReadOrganization(Organization.Identifier) ?? Organization;
        }
    }

    private async Task EmployeePropertiesClicked(Employee employee)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Employee Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<EmployeePropertiesDialog>(employee, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateEmployee((Employee)result.Data);
            Organization = await AuthorizationRegistry.ReadOrganization(Organization.Identifier) ?? Organization;
        }
    }

    private async Task EmployeeDeleteClicked(Employee employee)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {employee.GivenName} {employee.FamilyName}?",
            "Delete",
            "Cancel",
            "Delete Employee");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeleteEmployee(employee.EmployeeId);
            Organization = await AuthorizationRegistry.ReadOrganization(Organization.Identifier) ?? Organization;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing) StateContainer!.OnChange -= StateHasChanged;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
