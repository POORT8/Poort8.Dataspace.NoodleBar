﻿using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.CoreManager.OROrganizations.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.OROrganizations;

public partial class Details : ComponentBase, IDisposable
{
    private bool disposedValue;
    [Parameter]
    public required string Identifier { get; set; }
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

    protected override void OnInitialized()
    {
        StateContainer.OnChange += StateHasChanged;
    }

    private async Task EditOrganizationClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Organization",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditOrganizationClicked)
        };

        await DialogService.ShowDialogAsync<OrganizationDialog>(StateContainer.CurrentOROrganization!, parameters);
    }

    private async Task HandleEditOrganizationClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            var organization = (Organization)result.Data;
            StateContainer.CurrentOROrganization = await OrganizationRegistry.UpdateOrganization(organization);
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();

            var arOrganization = await AuthorizationRegistry.ReadOrganization(organization.Identifier);
            if (arOrganization is not null)
            {
                arOrganization.Name = organization.Name;
                arOrganization = await AuthorizationRegistry.UpdateOrganization(arOrganization);
                if (arOrganization.Identifier == StateContainer.CurrentAROrganization?.Identifier) StateContainer.CurrentAROrganization = arOrganization;
            }
        }
        else
        {
            StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
            StateContainer.CurrentOROrganization = StateContainer.CurrentOROrganizations.First(o => o.Identifier == StateContainer.CurrentOROrganization!.Identifier);
        }
    }

    private void BackClicked()
    {
        NavigationManager!.NavigateTo($"/or/organizations");
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