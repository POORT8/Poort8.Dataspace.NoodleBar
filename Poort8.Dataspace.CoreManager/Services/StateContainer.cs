namespace Poort8.Dataspace.CoreManager.Services;

public class StateContainer
{
    private AuthorizationRegistry.Entities.Organization? currentAROrganization;

    public AuthorizationRegistry.Entities.Organization? CurrentAROrganization
    {
        get => currentAROrganization;
        set
        {
            currentAROrganization = value;
            NotifyStateChanged();
        }
    }

    private OrganizationRegistry.Organization? currentOROrganization;

    public OrganizationRegistry.Organization? CurrentOROrganization
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
