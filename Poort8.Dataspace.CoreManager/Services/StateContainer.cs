using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Services;

public class StateContainer
{
    private Organization? currentOrganization;

    public Organization? CurrentOrganization
    {
        get => currentOrganization;
        set
        {
            currentOrganization = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();
}
