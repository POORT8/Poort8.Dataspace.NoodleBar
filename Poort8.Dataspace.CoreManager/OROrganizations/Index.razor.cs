using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations;

public partial class Index : ComponentBase, IDisposable
{
    private bool disposedValue;
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IOrganizationRegistry OrganizationRegistry { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;

        StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
    }

    private async Task AddNewClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Organization",
            PrimaryAction = "Add New Organization",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var organization = new Organization("", "");
        await DialogService.ShowDialogAsync<OrganizationDialog>(organization, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            var organization = (Organization)result.Data;
            await OrganizationRegistry.CreateOrganization(organization);
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();

            await UpdateAROrganizationName(organization.Identifier, organization.Name);
        }
    }

    private void DetailsClicked(Organization organization)
    {
        StateContainer.CurrentOROrganization = organization;
        NavigationManager.NavigateTo($"/or/organizations/details/{organization.Identifier}");
    }

    private async Task DeleteClicked(Organization organization)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {organization.Name}?",
            "Delete",
            "Cancel",
            "Delete Organization");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await OrganizationRegistry.DeleteOrganization(organization.Identifier);
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();

            await UpdateAROrganizationName(organization.Identifier);
        }
    }

    private async Task UpdateAROrganizationName(string identifier, string? newName = null)
    {
        var arOrganization = await AuthorizationRegistry.ReadOrganization(identifier);
        if (arOrganization is not null)
        {
            arOrganization.Name = newName ?? string.Empty;
            arOrganization = await AuthorizationRegistry.UpdateOrganization(arOrganization);
            if (arOrganization.Identifier == StateContainer.CurrentAROrganization?.Identifier) StateContainer.CurrentAROrganization = arOrganization;
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