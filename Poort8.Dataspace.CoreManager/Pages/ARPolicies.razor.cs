using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.Extensions;

namespace Poort8.Dataspace.CoreManager.Pages;

public partial class ARPolicies
{
    [Inject] private IAuthorizationRegistry? AuthorizationRegistryService { get; set; }
    [Inject] private ILogger<ARPolicies>? Logger { get; set; }
    private List<Policy>? _policies = new();
    private Policy? _selectedPolicy;
    private Policy? _newPolicy;
    private Policy? EditedPolicy => _selectedPolicy ?? _newPolicy;
    private Policy.PolicyProperty _policyProperty = new(string.Empty, string.Empty);
    private static bool DisableUpdatePolicy(Policy policy) =>
        string.IsNullOrWhiteSpace(policy.UseCase) ||
        string.IsNullOrWhiteSpace(policy.IssuerId) ||
        string.IsNullOrWhiteSpace(policy.SubjectId) ||
        string.IsNullOrWhiteSpace(policy.ResourceId) ||
        string.IsNullOrWhiteSpace(policy.Action) ||
        policy.IssuedAt < DateTimeOffset.MinValue.ToUnixTimeSeconds() ||
        policy.NotBefore < DateTimeOffset.MinValue.ToUnixTimeSeconds() ||
        policy.Expiration < DateTimeOffset.MinValue.ToUnixTimeSeconds() ||
        policy.IssuedAt > DateTimeOffset.MaxValue.ToUnixTimeSeconds() ||
        policy.NotBefore > DateTimeOffset.MaxValue.ToUnixTimeSeconds() ||
        policy.Expiration > DateTimeOffset.MaxValue.ToUnixTimeSeconds();
    private static bool DisableCreatePolicy(Policy policy) => DisableUpdatePolicy(policy);
    private DateTime policyNotBefore { get => DateTimeOffset.FromUnixTimeSeconds(EditedPolicy!.NotBefore).LocalDateTime; set => EditedPolicy!.NotBefore = ((DateTimeOffset)value).ToUnixTimeSeconds(); }
    private DateTime policyExpiration { get => DateTimeOffset.FromUnixTimeSeconds(EditedPolicy!.Expiration).LocalDateTime; set => EditedPolicy!.Expiration = ((DateTimeOffset)value).ToUnixTimeSeconds(); }

    protected override async Task OnInitializedAsync()
    {
        _policies = (await AuthorizationRegistryService!.ReadPolicies()).ToList();

        _ = base.OnInitializedAsync();
    }

    private void OnPolicyRowClick(Policy policy)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Clicked on row with policy {identifier} ({issuer}, {subject}, {resource}, {action})", policy.PolicyId, policy.IssuerId, policy.SubjectId, policy.ResourceId, policy.Action);
        _selectedPolicy = _policies?.FirstOrDefault(p => p.PolicyId.Equals(policy.PolicyId, StringComparison.OrdinalIgnoreCase))?.DeepCopy();
        _newPolicy = null;
    }

    private void AddNewPolicy()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - AddNewPolicy button clicked");
        _newPolicy = new Policy(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        _selectedPolicy = null;
        StateHasChanged();
    }

    private async Task CreatePolicy()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Create button clicked for policy {identifier} ({issuer}, {subject}, {resource}, {action})", _newPolicy!.PolicyId, _newPolicy.IssuerId, _newPolicy.SubjectId, _newPolicy.ResourceId, _newPolicy.Action);
        _newPolicy!.IssuedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
        var policy = await AuthorizationRegistryService!.CreatePolicy(_newPolicy!);
        _policies?.Add(policy);
        _newPolicy = null;
        StateHasChanged();
    }

    private async Task DeletePolicy(Policy policy)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Delete button clicked for policy {identifier} ({issuer}, {subject}, {resource}, {action})", policy.PolicyId, policy.IssuerId, policy.SubjectId, policy.ResourceId, policy.Action);
        await AuthorizationRegistryService!.DeletePolicy(policy.PolicyId);
        _policies?.Remove(policy);
        StateHasChanged();
    }

    private async Task UpdatePolicy()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Update button clicked for policy {identifier} ({issuer}, {subject}, {resource}, {action})", _selectedPolicy!.PolicyId, _selectedPolicy.IssuerId, _selectedPolicy.SubjectId, _selectedPolicy.ResourceId, _selectedPolicy.Action);
        _selectedPolicy!.IssuedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
        var policy = await AuthorizationRegistryService!.UpdatePolicy(_selectedPolicy!);
        _policies?.RemoveAll(p => p.PolicyId.Equals(policy.PolicyId, StringComparison.OrdinalIgnoreCase));
        _policies?.Add(policy);
        _selectedPolicy = null;
        StateHasChanged();
    }

    private void AddPolicyProperty()
    {
        EditedPolicy!.Properties.Add(_policyProperty);
        _policyProperty = new(string.Empty, string.Empty);
    }
}