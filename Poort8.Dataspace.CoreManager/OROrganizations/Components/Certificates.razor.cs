using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations.Components;

public partial class Certificates
{
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
            Title = $"New Certificate",
            PrimaryAction = "Add New Certificate",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var certificate = new Certificate(Array.Empty<byte>(), DateOnly.FromDateTime(DateTime.Now));
        await DialogService.ShowDialogAsync<CertificateDialog>(certificate, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.AddNewCertificateToOrganization(StateContainer.CurrentOROrganization!.Identifier, (Certificate)result.Data);
            StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
        }
    }

    private async Task EditClicked(Certificate certificate)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Certificate",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<CertificateDialog>(certificate, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await OrganizationRegistry.UpdateCertificate((Certificate)result.Data);
        }
        StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
    }

    private async Task DeleteClicked(Certificate certificate)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {certificate.CertificateId}?",
            "Delete",
            "Cancel",
            "Delete Certificate");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await OrganizationRegistry.DeleteCertificate(certificate.CertificateId);
            StateContainer.CurrentOROrganization = await OrganizationRegistry.ReadOrganization(StateContainer.CurrentOROrganization!.Identifier);
        }
    }
}