using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations.Components;

public partial class AuthorizationRegistries : ComponentBase, IDisposable
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
            Title = $"New Authorization Register",
            PrimaryAction = "Add New Authorization Register",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var authorizationRegistry = new OrganizationRegistry.AuthorizationRegistry(string.Empty, string.Empty);
        await DialogService.ShowDialogAsync<AuthorizationRegistryDialog>(authorizationRegistry, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.AddNewAuthorizationRegistryToOrganization(StateContainer.CurrentOROrganization!.Identifier, (OrganizationRegistry.AuthorizationRegistry)result.Data);
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
            StateContainer.CurrentOROrganization = StateContainer.CurrentOROrganizations.First(o => o.Identifier == StateContainer.CurrentOROrganization!.Identifier);
        }
    }

    private async Task EditClicked(OrganizationRegistry.AuthorizationRegistry authorizationRegistry)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Authorization Register",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<AuthorizationRegistryDialog>(authorizationRegistry, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.UpdateAuthorizationRegistry((OrganizationRegistry.AuthorizationRegistry)result.Data);
        }
        StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
        StateContainer.CurrentOROrganization = StateContainer.CurrentOROrganizations.First(o => o.Identifier == StateContainer.CurrentOROrganization!.Identifier);
    }

    private async Task DeleteClicked(OrganizationRegistry.AuthorizationRegistry authorizationRegistry)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {authorizationRegistry.AuthorizationRegistryOrganizationId}?",
            "Delete",
            "Cancel",
            "Delete Authorization Register");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await OrganizationRegistry.DeleteAuthorizationRegistry(authorizationRegistry.AuthorizationRegistryId);
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