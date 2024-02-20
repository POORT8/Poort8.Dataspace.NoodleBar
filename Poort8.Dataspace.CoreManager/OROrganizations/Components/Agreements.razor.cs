using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations.Components;

public partial class Agreements
{
    [Parameter]
    public ICollection<Agreement>? AgreementsCollection { get; set; }
    [Parameter]
    public EventCallback<Agreement> AgreementAdded { get; set; }
    [Parameter]
    public EventCallback<Agreement?> AgreementEdited { get; set; }
    [Parameter]
    public EventCallback<string> AgreementDeleted { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }

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

    private Task HandleAddNewAgreementClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            _ = AgreementAdded.InvokeAsync((Agreement)result.Data);
        }
        return Task.CompletedTask;
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

    private Task HandleEditAgreementClicked(DialogResult result)
    {
        _ = AgreementEdited.InvokeAsync(result.Cancelled ? null : (Agreement?)result.Data);
        return Task.CompletedTask;
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
            _ = AgreementDeleted.InvokeAsync(agreement.AgreementId);
        }
    }
}