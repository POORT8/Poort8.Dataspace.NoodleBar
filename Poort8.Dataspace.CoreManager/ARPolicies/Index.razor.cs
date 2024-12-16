using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.ARPolicies.Dialogs;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.ARPolicies;

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

    private IReadOnlyList<Policy>? Policies;

    private bool NoOrganizations => !(StateContainer.CurrentOROrganizations?.Count > 0);

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;

        Policies = await AuthorizationRegistry.ReadPolicies();

        StateContainer.CurrentOROrganizations = await OrganizationRegistry.ReadOrganizations();
    }

    private async Task AddNewClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Policy",
            PrimaryAction = "Add New Policy",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var policy = new Policy(Options.Value.UseCase, StateContainer.CurrentOROrganizations![0].Identifier, StateContainer.CurrentOROrganizations[0].Identifier, string.Empty, string.Empty)
        {
            ServiceProvider = StateContainer.CurrentOROrganizations[0].Identifier,
            NotBefore = ((DateTimeOffset)DateTimeOffset.Now.Date).ToUnixTimeSeconds(),
            Expiration = ((DateTimeOffset)DateTimeOffset.Now.AddYears(1).Date).ToUnixTimeSeconds()
        };
        await DialogService.ShowDialogAsync<PolicyDialog>(policy, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            var policy = (Policy)result.Data;
            policy.IssuedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
            await AuthorizationRegistry.CreatePolicy(policy);
            Policies = await AuthorizationRegistry.ReadPolicies();
        }
    }

    private void DetailsClicked(Policy policy)
    {
        StateContainer.CurrentPolicy = policy;
        NavigationManager.NavigateTo($"/ar/policies/details/{policy.PolicyId}");
    }

    private async Task PropertiesClicked(Policy policy)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Policy Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<PolicyPropertiesDialog>(policy, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdatePolicy((Policy)result.Data);
        }
        Policies = await AuthorizationRegistry.ReadPolicies();
    }

    private async Task DeleteClicked(Policy policy)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {policy.PolicyId}?",
            "Delete",
            "Cancel",
            "Delete Policy");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeletePolicy(policy.PolicyId);
            Policies = await AuthorizationRegistry.ReadPolicies();
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
