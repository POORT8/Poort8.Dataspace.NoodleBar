using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations.Components;

public partial class Agreements
{
    [Parameter]
    public ICollection<Agreement>? AgreementsCollection { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required IOrganizationRegistry OrganizationRegistry { get; set; }
    [Inject]
    public required StateContainer StateContainer { get; set; }

    private async Task AddNewAgreementClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Agreement",
            PrimaryAction = "Add New Agreement",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewAgreementClicked)
        };

        var agreement = new Agreement(string.Empty, string.Empty, string.Empty, DateOnly.FromDateTime(DateTime.Now), DateOnly.MaxValue, string.Empty, Array.Empty<byte>(), string.Empty, null);
        await DialogService.ShowDialogAsync<AgreementDialog>(agreement, parameters);
    }

    private async Task HandleAddNewAgreementClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.AddNewAgreementToOrganization(StateContainer.CurrentOROrganization!.Identifier, (Agreement)result.Data);
            StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
        }
    }

    private async Task AgreementEditClicked(Agreement agreement)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Agreement",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditAgreementClicked)
        };

        await DialogService.ShowDialogAsync<AgreementDialog>(agreement, parameters);
    }

    private async Task HandleEditAgreementClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.UpdateAgreement((Agreement)result.Data);
        }
        StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
    }

    private async Task AgreementDeleteClicked(Agreement agreement)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {agreement.Type} {agreement.Title}?",
            "Delete",
            "Cancel",
            "Delete Agreement");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await OrganizationRegistry.DeleteAgreement(agreement.AgreementId);
            StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
        }
    }
}