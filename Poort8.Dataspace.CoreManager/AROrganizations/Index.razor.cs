using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.CoreManager.AROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;
using AROrganization = Poort8.Dataspace.AuthorizationRegistry.Entities.Organization;
using OROrganization = Poort8.Dataspace.OrganizationRegistry.Organization;

namespace Poort8.Dataspace.CoreManager.AROrganizations;

public partial class Index : ComponentBase, IDisposable
{
    private bool disposedValue;
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IOrganizationRegistry OrganizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required IOptions<CoreManagerOptions> Options { get; set; }

    private IReadOnlyList<AROrganization>? AROrganizations;
    private IReadOnlyList<OROrganization>? NotAddedOROrganizations;

    private bool NoOrganizations => !(NotAddedOROrganizations?.Count > 0);

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;
        await FetchOrganizations();
    }

    private async Task FetchOrganizations()
    {
        AROrganizations = await AuthorizationRegistry.ReadOrganizations(Options.Value.UseCase);
        StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
        NotAddedOROrganizations = StateContainer.CurrentOROrganizations.Where(o => !AROrganizations.Any(a => a.Identifier == o.Identifier)).ToList();
    }

    private async Task AddNewClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Add Organization",
            PrimaryAction = "Add Organization",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var organization = new AROrganization(NotAddedOROrganizations![0].Identifier, Options.Value.UseCase, NotAddedOROrganizations[0].Name, "", "", "");
        await DialogService.ShowDialogAsync<AddOrganizationDialog>((organization, NotAddedOROrganizations), parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.CreateOrganization((((AROrganization AROrganization, IReadOnlyList<OROrganization>))result.Data).AROrganization);
            await FetchOrganizations();
        }
    }

    private void DetailsClicked(AROrganization organization)
    {
        StateContainer.CurrentAROrganization = organization;
        NavigationManager.NavigateTo($"/ar/organizations/details/{organization.Identifier}");
    }

    private async Task PropertiesClicked(AROrganization organization)
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
            await AuthorizationRegistry.UpdateOrganization((AROrganization)result.Data);
        }
        AROrganizations = await AuthorizationRegistry.ReadOrganizations(Options.Value.UseCase);
    }

    private async Task DeleteClicked(AROrganization organization)
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
            await FetchOrganizations();
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
