using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.ARResourceGroups.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.ARResourceGroups;

public partial class Details : ComponentBase, IDisposable
{
    private bool disposedValue;
    [Parameter]
    public required string ResourceGroupId { get; set; }
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

    private IReadOnlyList<Resource>? Resources;

    protected async override Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;

        Resources = await AuthorizationRegistry.ReadResources(Options.Value.UseCase);
    }

    private async Task EditClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit {Options.Value.ResourceGroupAlternativeName}",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<ResourceGroupDialog>(StateContainer.CurrentResourceGroup!, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            StateContainer.CurrentResourceGroup = await AuthorizationRegistry.UpdateResourceGroup((ResourceGroup)result.Data);
        }
        else
        {
            StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
        }
    }

    private void BackClicked()
    {
        NavigationManager!.NavigateTo($"/ar/resourceGroups");
    }

    private async Task AddNewResourceClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New {Options.Value.ResourceAlternativeName}",
            PrimaryAction = $"Add New {Options.Value.ResourceAlternativeName}",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewResourceClicked)
        };

        var resource = new Resource("", Options.Value.UseCase, "", "");
        await DialogService.ShowDialogAsync<NewResourceDialog>(resource, parameters);
    }

    private async Task HandleAddNewResourceClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.AddNewResourceToResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId, (Resource)result.Data);
            StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
            Resources = await AuthorizationRegistry.ReadResources(Options.Value.UseCase);
        }
    }

    private async Task AddExistingResourcesClicked()
    {
        var resourceEntryList = Resources?
            .Where(f => !StateContainer.CurrentResourceGroup!.Resources.Any(pf => pf.ResourceId == f.ResourceId))
            .Select(f => (Resource: f, Add: false))
            .OrderBy(t => t.Resource.Name).ToList() ?? new List<(Resource, bool)>();

        var parameters = new DialogParameters()
        {
            Title = $"Existing {Options.Value.ResourceAlternativeNamePlural}",
            PrimaryAction = $"Add Existing {Options.Value.ResourceAlternativeNamePlural}",
            PrimaryActionEnabled = resourceEntryList.Count > 0,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddExistingResourcesClicked)
        };

        await DialogService.ShowDialogAsync<ExistingResourcesDialog>(resourceEntryList, parameters);
    }

    private async Task HandleAddExistingResourcesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            foreach (var resourceId in ((List<(Resource Resource, bool Add)>)result.Data).Where(o => o.Add).Select(o => o.Resource.ResourceId))
                await AuthorizationRegistry.AddExistingResourceToResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId, resourceId);

            StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
        }
    }

    private async Task ResourceEditClicked(Resource resource)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit {Options.Value.ResourceAlternativeName}",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditResourceClicked)
        };

        await DialogService.ShowDialogAsync<NewResourceDialog>(resource, parameters);
    }

    private async Task HandleEditResourceClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateResource((Resource)result.Data);
        }
        StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
        Resources = await AuthorizationRegistry.ReadResources(Options.Value.UseCase);
    }

    private async Task ResourcePropertiesClicked(Resource resource)
    {
        var parameters = new DialogParameters()
        {
            Title = $"{Options.Value.ResourceAlternativeName} Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<ResourcePropertiesDialog>(resource, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateResource((Resource)result.Data);
        }
        StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
    }

    private async Task ResourceRemoveClicked(Resource resource)
    {
        await AuthorizationRegistry.RemoveResourceFromResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId, resource.ResourceId);
        StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
    }

    private async Task ResourceDeleteClicked(Resource resource)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {resource.Name}?",
            "Delete",
            "Cancel",
            $"Delete {Options.Value.ResourceAlternativeName}");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeleteResource(resource.ResourceId);
            StateContainer.CurrentResourceGroup = await AuthorizationRegistry.ReadResourceGroup(StateContainer.CurrentResourceGroup!.ResourceGroupId);
            Resources = await AuthorizationRegistry.ReadResources(Options.Value.UseCase);
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