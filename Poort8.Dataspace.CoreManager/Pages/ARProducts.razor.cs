using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Pages;

public partial class ARProducts
{
    [Inject] private IAuthorizationRegistry? AuthorizationRegistryService { get; set; }
    [Inject] private ILogger<ARProducts>? Logger { get; set; }
    private List<Product>? _products = new();
    private Product? _selectedProduct;
    private Product? _newProduct;
    private Product? EditedProduct => _selectedProduct ?? _newProduct;
    private Property _productProperty = new(string.Empty, string.Empty);
    private static bool DisableUpdateProduct(Product product) => string.IsNullOrWhiteSpace(product.Name);
    private bool DisableCreateProduct(Product product) => DisableUpdateProduct(product) || string.IsNullOrWhiteSpace(product.ProductId) || _products?.Any(p => product.ProductId.Equals(p.ProductId, StringComparison.OrdinalIgnoreCase)) == true;
    private List<Feature>? _features = new();
    private Feature? _selectedFeature;
    private Feature? _newFeature;
    private Feature? EditedFeature => _selectedFeature ?? _newFeature;
    private Property _featureProperty = new(string.Empty, string.Empty);
    private static bool DisableUpdateFeature(Feature feature) => string.IsNullOrWhiteSpace(feature.Name) || !feature.Products.Any();
    private bool DisableCreateFeature(Feature feature) => DisableUpdateFeature(feature) || string.IsNullOrWhiteSpace(feature.FeatureId) || _features?.Any(f => feature.FeatureId.Equals(f.FeatureId, StringComparison.OrdinalIgnoreCase)) == true || feature.Products.Count != 1;
    private string _featureProductId = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _products = (await AuthorizationRegistryService!.ReadProducts()).ToList();
        _features = (await AuthorizationRegistryService.ReadFeatures()).ToList();

        _ = base.OnInitializedAsync();
    }

    private void OnProductRowClick(Product product)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Clicked on row with product {identifier} ({name})", product!.ProductId, product.Name);
        _selectedProduct = product.DeepCopy();
        _newProduct = null;
    }

    private void OnFeatureRowClick(Feature feature)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Clicked on row with feature {identifier} ({name})", feature!.FeatureId, feature.Name);
        _selectedFeature = feature.DeepCopy();
        _newFeature = null;
    }

    private void AddNewProduct()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - AddNewProduct button clicked");
        _newProduct = new Product(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        _selectedProduct = null;
        StateHasChanged();
    }

    private void AddNewFeature()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - AddNewFeature button clicked");
        _newFeature = new Feature(string.Empty, string.Empty, string.Empty);
        _selectedFeature = null;
        StateHasChanged();
    }

    private async Task CreateProduct()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Create button clicked for product {identifier} ({name})", _newProduct!.ProductId, _newProduct.Name);
        var product = await AuthorizationRegistryService!.CreateProduct(_newProduct!);
        _products?.Add(product);
        _newProduct = null;
        StateHasChanged();
    }

    private async Task DeleteProduct()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Delete button clicked for product {identifier} ({name})", _selectedProduct!.ProductId, _selectedProduct.Name);
        await AuthorizationRegistryService!.DeleteProduct(_selectedProduct!.ProductId);
        _products?.RemoveAll(p => p.ProductId.Equals(_selectedProduct!.ProductId, StringComparison.OrdinalIgnoreCase));
        _selectedProduct = null;
        StateHasChanged();
    }

    private async Task UpdateProduct()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Update button clicked for product {identifier} ({name})", _selectedProduct!.ProductId, _selectedProduct.Name);
        var product = await AuthorizationRegistryService!.UpdateProduct(_selectedProduct!);
        _products?.RemoveAll(p => p.ProductId.Equals(product.ProductId, StringComparison.OrdinalIgnoreCase));
        _products?.Add(product);
        _selectedProduct = null;
        StateHasChanged();
    }

    private async Task CreateFeature()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Create button clicked for feature {identifier} ({name})", _newFeature!.FeatureId, _newFeature.Name);
        var feature = await AuthorizationRegistryService!.AddFeature(_newFeature!.Products.First().ProductId, _newFeature);
        _features?.Add(feature);
        _newFeature = null;
        StateHasChanged();
    }

    private async Task DeleteFeature()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Delete button clicked for feature {identifier} ({name})", _selectedFeature!.FeatureId, _selectedFeature.Name);
        await AuthorizationRegistryService!.DeleteFeature(_selectedFeature!.FeatureId);
        _features?.RemoveAll(f => f.FeatureId.Equals(_selectedFeature!.FeatureId, StringComparison.OrdinalIgnoreCase));
        _selectedFeature = null;
        StateHasChanged();
    }

    private async Task UpdateFeature()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Update button clicked for feature {identifier} ({name})", _selectedFeature!.FeatureId, _selectedFeature.Name);
        var feature = await AuthorizationRegistryService!.UpdateFeature(_selectedFeature!);
        _features?.RemoveAll(f => f.FeatureId.Equals(feature.FeatureId, StringComparison.OrdinalIgnoreCase));
        _features?.Add(feature);
        _selectedFeature = null;
        StateHasChanged();
    }

    private void ResetProductProperty()
    {
        _productProperty = new(string.Empty, string.Empty);
    }

    private void AddProductProperty()
    {
        EditedProduct!.Properties.Add(_productProperty);
        ResetProductProperty();
    }

    private void DeleteProductProperty()
    {
        EditedProduct!.Properties.Remove(EditedProduct.Properties.First(p => p.Key.Equals(_productProperty.Key, StringComparison.OrdinalIgnoreCase)));
        ResetProductProperty();
    }

    private void AddFeatureProduct()
    {
        EditedFeature!.Products.Add(_products!.First(p => p.ProductId.Equals(_featureProductId, StringComparison.OrdinalIgnoreCase)));
        _featureProductId = string.Empty;
    }

    private void DeleteFeatureProduct()
    {
        EditedFeature!.Products.Remove(EditedFeature.Products.First(p => p.ProductId.Equals(_featureProductId, StringComparison.OrdinalIgnoreCase)));
        _featureProductId = string.Empty;
    }

    private void ResetFeatureProperty()
    {
        _featureProperty = new(string.Empty, string.Empty);
    }

    private void AddFeatureProperty()
    {
        EditedFeature!.Properties.Add(_featureProperty);
        ResetFeatureProperty();
    }

    private void DeleteFeatureProperty()
    {
        EditedFeature!.Properties.Remove(EditedFeature.Properties.First(p => p.Key.Equals(_featureProperty.Key, StringComparison.OrdinalIgnoreCase)));
        ResetFeatureProperty();
    }
}