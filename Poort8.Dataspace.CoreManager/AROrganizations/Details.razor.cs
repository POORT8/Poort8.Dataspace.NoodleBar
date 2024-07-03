using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.AROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.AROrganizations;

public partial class Details : ComponentBase, IDisposable
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
    [Inject]
    public required IOptions<CoreManagerOptions> Options { get; set; }

    protected override void OnInitialized()
    {
        StateContainer.OnChange += StateHasChanged;
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

        await DialogService.ShowDialogAsync<EditOrganizationDialog>(StateContainer.CurrentAROrganization!, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            StateContainer.CurrentAROrganization = await AuthorizationRegistry.UpdateOrganization((Organization)result.Data);
        }
        else
        {
            StateContainer.CurrentAROrganization = await AuthorizationRegistry.ReadOrganization(StateContainer.CurrentAROrganization!.Identifier);
        }
    }

    private void BackClicked()
    {
        NavigationManager!.NavigateTo($"/ar/organizations");
    }

    private async Task AddNewEmployeeClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New {Options.Value.EmployeeAlternativeName}",
            PrimaryAction = $"Add New {Options.Value.EmployeeAlternativeName}",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewEmployeeClicked)
        };

        var employee = new Employee("", Options.Value.UseCase, "", "", "", "");
        await DialogService.ShowDialogAsync<EmployeeDialog>(employee, parameters);
    }

    private async Task HandleAddNewEmployeeClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.AddNewEmployeeToOrganization(StateContainer.CurrentAROrganization!.Identifier, (Employee)result.Data);
            StateContainer.CurrentAROrganization = await AuthorizationRegistry.ReadOrganization(StateContainer.CurrentAROrganization!.Identifier);
        }
    }

    private async Task EmployeeEditClicked(Employee employee)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit {Options.Value.EmployeeAlternativeName}",
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
        }
        StateContainer.CurrentAROrganization = await AuthorizationRegistry.ReadOrganization(StateContainer.CurrentAROrganization!.Identifier);
    }

    private async Task EmployeePropertiesClicked(Employee employee)
    {
        var parameters = new DialogParameters()
        {
            Title = $"{Options.Value.EmployeeAlternativeName} Properties",
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
        }
        StateContainer.CurrentAROrganization = await AuthorizationRegistry.ReadOrganization(StateContainer.CurrentAROrganization!.Identifier);
    }

    private async Task EmployeeDeleteClicked(Employee employee)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {employee.GivenName} {employee.FamilyName}?",
            "Delete",
            "Cancel",
            $"Delete {Options.Value.EmployeeAlternativeName}");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeleteEmployee(employee.EmployeeId);
            StateContainer.CurrentAROrganization = await AuthorizationRegistry.ReadOrganization(StateContainer.CurrentAROrganization!.Identifier);
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
