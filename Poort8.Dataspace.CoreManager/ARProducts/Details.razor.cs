using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.ARProducts.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.ARProducts;

public partial class Details : IDisposable
{
    private bool disposedValue;
    [Parameter]
    public required string ProductId { get; set; }
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }
    public Product Product { get; private set; } = default!;
    public IQueryable<Feature>? Features;

    protected async override void OnInitialized()
    {
        StateContainer.OnChange += StateHasChanged;
        Product = StateContainer.CurrentProduct!;

        Features = (await AuthorizationRegistry!.ReadFeatures()).AsQueryable();
    }

    private async Task EditClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Product",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditClicked)
        };

        await DialogService.ShowDialogAsync<ProductDialog>(Product, parameters);
    }

    private async Task HandleEditClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            Product = await AuthorizationRegistry.UpdateProduct((Product)result.Data);
        }
    }

    private void BackClicked()
    {
        StateContainer.CurrentProduct = Product;
        NavigationManager!.NavigateTo($"/ar/products");
    }

    private async Task AddNewFeatureClicked() //TODO: To Feature component?
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Feature",
            PrimaryAction = "Add New Feature",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewFeatureClicked)
        };

        var feature = new Feature("", "", "");
        await DialogService.ShowDialogAsync<NewFeatureDialog>(feature, parameters);
    }

    private async Task HandleAddNewFeatureClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.AddNewFeatureToProduct(Product.ProductId, (Feature)result.Data);
            Product = await AuthorizationRegistry.ReadProduct(Product.ProductId) ?? Product;
        }
    }

    private async Task AddExistingFeatureClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Existing Feature",
            PrimaryAction = "Add Existing Feature",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddExistingFeatureClicked)
        };

        var existingFeatureList = Features?.Where(o => !Product.Features.Any(f => f.FeatureId == o.FeatureId)) ?? Enumerable.Empty<Feature>();
        List<Tuple<Feature, bool>> featureEntryList = [];

        if (existingFeatureList.Any())
            featureEntryList = existingFeatureList.Select(f => Tuple.Create(f, false)).OrderBy(e => e.Item1.Name).ToList();
        
        await DialogService.ShowDialogAsync<ExistingFeatureDialog>(featureEntryList, parameters);
    }

    private async Task HandleAddExistingFeatureClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            foreach (var feature in ((List<Tuple<Feature, bool>>)result.Data).Where(o => o.Item2).Select(o => o.Item1))
                await AuthorizationRegistry.AddExistingFeatureToProduct(Product.ProductId, feature.FeatureId);

            Product = await AuthorizationRegistry.ReadProduct(Product.ProductId) ?? Product;
        }
    }

    private async Task FeatureEditClicked(Feature feature)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Edit Feature",
            PrimaryAction = "Save Changes",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleEditFeatureClicked)
        };

        await DialogService.ShowDialogAsync<NewFeatureDialog>(feature, parameters);
    }

    private async Task HandleEditFeatureClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateFeature((Feature)result.Data);
            Product = await AuthorizationRegistry.ReadProduct(Product.ProductId) ?? Product;
        }
    }

    private async Task FeaturePropertiesClicked(Feature feature)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Feature Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<FeaturePropertiesDialog>(feature, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateFeature((Feature)result.Data);
            Product = await AuthorizationRegistry.ReadProduct(Product.ProductId) ?? Product;
        }
    }

    private async Task FeatureRemoveClicked(Feature feature)
    {
        await AuthorizationRegistry.RemoveFeatureFromProduct(Product.ProductId, feature.FeatureId);
        Product = await AuthorizationRegistry.ReadProduct(Product.ProductId) ?? Product;
    }

    private async Task FeatureDeleteClicked(Feature feature)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {feature.Name}?",
            "Delete",
            "Cancel",
            "Delete Feature");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeleteFeature(feature.FeatureId);
            Product = await AuthorizationRegistry.ReadProduct(Product.ProductId) ?? Product;
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
}
