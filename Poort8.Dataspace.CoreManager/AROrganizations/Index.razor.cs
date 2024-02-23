using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.AROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.AROrganizations;

public partial class Index : IDisposable
{
    private bool disposedValue;
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }

    public IQueryable<Organization>? Organizations;

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;

        Organizations = (await AuthorizationRegistry!.ReadOrganizations()).AsQueryable();
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

        var organization = new Organization("", "", "", "", "");
        await DialogService.ShowDialogAsync<OrganizationDialog>(organization, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.CreateOrganization((Organization)result.Data);
            Organizations = (await AuthorizationRegistry.ReadOrganizations()).AsQueryable();
        }
    }

    private void DetailsClicked(Organization organization)
    {
        StateContainer.CurrentAROrganization = organization;
        NavigationManager.NavigateTo($"/ar/organizations/details/{organization.Identifier}");
    }

    private async Task PropertiesClicked(Organization organization)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Organization Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<OrganizationPropertiesDialog>(organization, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateOrganization((Organization)result.Data);
            Organizations = (await AuthorizationRegistry.ReadOrganizations()).AsQueryable();
        }
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
            await AuthorizationRegistry.DeleteOrganization(organization.Identifier);
            Organizations = (await AuthorizationRegistry.ReadOrganizations()).AsQueryable();
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
