using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Services;

public class StateContainer
{
    private Organization? currentOrganization;
    private Product? currentProduct;

    public Organization? CurrentOrganization
    {
        get => currentOrganization;
        set
        {
            currentOrganization = value;
            NotifyStateChanged();
        }
    }

    public Product? CurrentProduct
    {
        get => currentProduct;
        set
        {
            currentProduct = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();
}
