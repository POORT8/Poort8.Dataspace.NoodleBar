namespace Poort8.Dataspace.CoreManager.Services;

public class StateContainer
{
    private AuthorizationRegistry.Entities.Organization? currentAROrganization;
    private AuthorizationRegistry.Entities.Product? currentProduct;
    private OrganizationRegistry.Organization? currentOROrganization;

    public AuthorizationRegistry.Entities.Organization? CurrentAROrganization
    {
        get => currentAROrganization;
        set
        {
            currentAROrganization = value;
            NotifyStateChanged();
        }
    }

    public AuthorizationRegistry.Entities.Product? CurrentProduct
    {
        get => currentProduct;
        set
        {
            currentProduct = value;
            NotifyStateChanged();
        }
    }

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
