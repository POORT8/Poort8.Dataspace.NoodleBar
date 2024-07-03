using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations.Components;

public partial class Roles : ComponentBase, IDisposable
{
    private bool disposedValue;
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required IOrganizationRegistry OrganizationRegistry { get; set; }
    [Inject]
    public required StateContainer StateContainer { get; set; }

    private async Task AddNewClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Role",
            PrimaryAction = "Add New Role",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var role = new OrganizationRole(string.Empty);
        await DialogService.ShowDialogAsync<OrganizationRoleDialog>(role, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.AddNewRoleToOrganization(StateContainer.CurrentOROrganization!.Identifier, (OrganizationRole)result.Data);
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
            StateContainer.CurrentOROrganization = StateContainer.CurrentOROrganizations.First(o => o.Identifier == StateContainer.CurrentOROrganization!.Identifier);
        }
    }

    private async Task EditClicked(OrganizationRole role)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Role",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<OrganizationRoleDialog>(role, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.UpdateRole((OrganizationRole)result.Data);
        }
        StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
        StateContainer.CurrentOROrganization = StateContainer.CurrentOROrganizations.First(o => o.Identifier == StateContainer.CurrentOROrganization!.Identifier);
    }

    private async Task DeleteClicked(OrganizationRole role)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {role.Role}?",
            "Delete",
            "Cancel",
            "Delete Role");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await OrganizationRegistry.DeleteRole(role.RoleId);
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
            StateContainer.CurrentOROrganization = StateContainer.CurrentOROrganizations.First(o => o.Identifier == StateContainer.CurrentOROrganization!.Identifier);
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