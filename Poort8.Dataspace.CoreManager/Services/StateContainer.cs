using Poort8.Dataspace.OrganizationRegistry;

namespace Poort8.Dataspace.CoreManager.Services;

public class StateContainer
{
    private AuthorizationRegistry.Entities.Organization? currentAROrganization;
    private AuthorizationRegistry.Entities.ResourceGroup? currentResourceGroup;
    private AuthorizationRegistry.Entities.Policy? currentPolicy;
    private IReadOnlyList<Organization>? currentOROrganizations;
    private Organization? currentOROrganization;

    public AuthorizationRegistry.Entities.Organization? CurrentAROrganization
    {
        get => currentAROrganization;
        set
        {
            currentAROrganization = value;
            NotifyStateChanged();
        }
    }

    public AuthorizationRegistry.Entities.ResourceGroup? CurrentResourceGroup
    {
        get => currentResourceGroup;
        set
        {
            currentResourceGroup = value;
            NotifyStateChanged();
        }
    }

    public AuthorizationRegistry.Entities.Policy? CurrentPolicy
    {
        get => currentPolicy;
        set
        {
            currentPolicy = value;
            NotifyStateChanged();
        }
    }

    public IReadOnlyList<Organization>? CurrentOROrganizations
    {
        get => currentOROrganizations;
        set
        {
            currentOROrganizations = value;
            NotifyStateChanged();
        }
    }

    public Organization? CurrentOROrganization
    {
        get => currentOROrganization;
        set
        {
            currentOROrganization = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();
}
