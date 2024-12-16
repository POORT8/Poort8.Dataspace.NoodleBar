using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.ARResourceGroups.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.ARResourceGroups;

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

    private IReadOnlyList<ResourceGroup>? ResourceGroups;

    private bool NoOrganizations => !(StateContainer.CurrentOROrganizations?.Count > 0);

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;

        ResourceGroups = await AuthorizationRegistry.ReadResourceGroups(Options.Value.UseCase);

        StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
    }

    private async Task AddNewClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New {Options.Value.ResourceGroupAlternativeName}",
            PrimaryAction = $"Add New {Options.Value.ResourceGroupAlternativeName}",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var resourceGroup = new ResourceGroup("", Options.Value.UseCase, "", "", StateContainer.CurrentOROrganizations![0].Identifier, "");
        await DialogService.ShowDialogAsync<ResourceGroupDialog>(resourceGroup, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.CreateResourceGroup((ResourceGroup)result.Data);
            ResourceGroups = await AuthorizationRegistry.ReadResourceGroups(Options.Value.UseCase);
        }
    }

    private void DetailsClicked(ResourceGroup resourceGroup)
    {
        StateContainer.CurrentResourceGroup = resourceGroup;
        NavigationManager.NavigateTo($"/ar/resourcegroups/details/{resourceGroup.ResourceGroupId}");
    }

    private async Task PropertiesClicked(ResourceGroup resourceGroup)
    {
        var parameters = new DialogParameters()
        {
            Title = $"{Options.Value.ResourceGroupAlternativeName} Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<ResourceGroupPropertiesDialog>(resourceGroup, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateResourceGroup((ResourceGroup)result.Data);
        }
        ResourceGroups = await AuthorizationRegistry.ReadResourceGroups(Options.Value.UseCase);
    }

    private async Task DeleteClicked(ResourceGroup resourceGroup)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {resourceGroup.Name}?",
            "Delete",
            "Cancel",
            $"Delete {Options.Value.ResourceGroupAlternativeName}");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeleteResourceGroup(resourceGroup.ResourceGroupId);
            ResourceGroups = await AuthorizationRegistry.ReadResourceGroups(Options.Value.UseCase);
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

    private string GetOROrganizationName(string? identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return string.Empty;
        else
            return (StateContainer.CurrentOROrganizations?.FirstOrDefault(o => o.Identifier == identifier)?.Name + " " ?? string.Empty) + $"({identifier})";
    }
}
