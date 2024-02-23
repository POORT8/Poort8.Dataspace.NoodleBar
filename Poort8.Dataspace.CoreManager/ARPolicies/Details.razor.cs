using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.ARPolicies.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.ARPolicies;

public partial class Details : IDisposable
{
    private bool disposedValue;
    [Parameter]
    public required string Identifier { get; set; }
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }

    protected override void OnInitialized()
    {
        StateContainer.OnChange += StateHasChanged;
    }

    private async Task EditClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Policy",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<PolicyDialog>(StateContainer.CurrentPolicy!, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            var policy = (Policy)result.Data;
            policy.IssuedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
            StateContainer.CurrentPolicy = await AuthorizationRegistry.UpdatePolicy(policy);
        }
        else
        {
            StateContainer.CurrentPolicy = await AuthorizationRegistry.ReadPolicy(StateContainer.CurrentPolicy!.PolicyId);
        }
    }

    private void BackClicked()
    {
        NavigationManager!.NavigateTo($"/ar/policies");
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