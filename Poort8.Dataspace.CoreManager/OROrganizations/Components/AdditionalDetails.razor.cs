using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations.Components;

public partial class AdditionalDetails
{
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required IOrganizationRegistry OrganizationRegistry { get; set; }
    [Inject]
    public required StateContainer StateContainer { get; set; }

    private async Task EditClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit additional details",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<OrganizationAdditionalDetailsDialog>(StateContainer.CurrentOROrganization!, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            StateContainer.CurrentOROrganization = await OrganizationRegistry.UpdateOrganization((Organization)result.Data);
        }
        else
        {
            StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
        }
    }
}