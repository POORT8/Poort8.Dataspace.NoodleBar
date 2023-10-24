using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;

namespace Poort8.Dataspace.CoreManager.Pages;

public partial class AuthorizationRegistry
{
    [Inject] private IAuthorizationRegistry? AuthorizationRegistryService { get; set; }
    [Inject] private ILogger<AuthorizationRegistry>? Logger { get; set; }
    private List<Organization>? _organizations = new();
    private Organization? _selectedOrganization;
    private Organization? _newOrganization;
    private Organization? EditedOrganization => _selectedOrganization ?? _newOrganization;
    private Employee _employee = new();
    private Property _organizationProperty = new(string.Empty, string.Empty);
    private static bool DisableUpdateOrganization(Organization organization) => string.IsNullOrWhiteSpace(organization.Name);
    private bool DisableCreateOrganization(Organization organization) => DisableUpdateOrganization(organization) || string.IsNullOrWhiteSpace(organization.Identifier) || _organizations?.Any(o => organization.Identifier.Equals(o.Identifier, StringComparison.OrdinalIgnoreCase)) == true;
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
    private List<Policy>? _policies = new();
    private Policy? _selectedPolicy;
    private Policy? _newPolicy;
    private Policy? EditedPolicy => _selectedPolicy ?? _newPolicy;
    private Property _policyProperty = new(string.Empty, string.Empty);
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
        _organizations = (await AuthorizationRegistryService!.ReadOrganizations()).ToList();
        _products = (await AuthorizationRegistryService.ReadProducts()).ToList();
        _features = (await AuthorizationRegistryService.ReadFeatures()).ToList();
        _policies = (await AuthorizationRegistryService.ReadPolicies()).ToList(); 

        _ = base.OnInitializedAsync();
    }

    private void OnOrganizationRowClick(Organization organization)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Clicked on row with organization {identifier} ({name})", organization!.Identifier, organization.Name);
        _selectedOrganization = organization.DeepCopy();
        _newOrganization = null;
        ResetEmployee();
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

    private void OnPolicyRowClick(Policy policy)
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Clicked on row with policy {identifier} ({issuer}, {subject}, {resource}, {action})", policy!.PolicyId, policy.IssuerId, policy.SubjectId, policy.ResourceId, policy.Action);
        _selectedPolicy = policy.DeepCopy();
        _newPolicy = null;
    }

    private void AddNewOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - AddNewOrganization button clicked");
        _newOrganization = new Organization(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        _selectedOrganization = null;
        ResetEmployee();
        StateHasChanged();
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

    private void AddNewPolicy()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - AddNewPolicy button clicked");
        _newPolicy = new Policy(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        _selectedPolicy = null;
        StateHasChanged();
    }

    private async Task CreateOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Create button clicked for organization {identifier} ({name})", _newOrganization!.Identifier, _newOrganization.Name);
        var organization = await AuthorizationRegistryService!.CreateOrganization(_newOrganization!);
        _organizations?.Add(organization);
        _newOrganization = null;
        StateHasChanged();
    }

    private async Task DeleteOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Delete button clicked for organization {identifier} ({name})", _selectedOrganization!.Identifier, _selectedOrganization.Name);
        await AuthorizationRegistryService!.DeleteOrganization(_selectedOrganization!.Identifier);
        _organizations?.RemoveAll(p => p.Identifier.Equals(_selectedOrganization!.Identifier, StringComparison.OrdinalIgnoreCase));
        _selectedOrganization = null;
        StateHasChanged();
    }

    private async Task UpdateOrganization()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Update button clicked for organization {identifier} ({name})", _selectedOrganization!.Identifier, _selectedOrganization.Name);
        var party = await AuthorizationRegistryService!.UpdateOrganization(_selectedOrganization!);
        _organizations?.RemoveAll(p => p.Identifier.Equals(party.Identifier, StringComparison.OrdinalIgnoreCase));
        _organizations?.Add(party);
        _selectedOrganization = null;
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

    private async Task CreatePolicy()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Create button clicked for policy {identifier} ({issuer}, {subject}, {resource}, {action})", _newPolicy!.PolicyId, _newPolicy.IssuerId, _newPolicy.SubjectId, _newPolicy.ResourceId, _newPolicy.Action);
        _newPolicy!.IssuedAt = DateTimeOffset.Now.ToUnixTimeSeconds();
        var policy = await AuthorizationRegistryService!.CreatePolicy(_newPolicy!);
        _policies?.Add(policy);
        _newPolicy = null;
        StateHasChanged();
    }

    private async Task DeletePolicy()
    {
        Logger?.LogInformation("P8.inf - AuthorizationRegistry - Delete button clicked for policy {identifier} ({issuer}, {subject}, {resource}, {action})", _selectedPolicy!.PolicyId, _selectedPolicy.IssuerId, _selectedPolicy.SubjectId, _selectedPolicy.ResourceId, _selectedPolicy.Action);
        await AuthorizationRegistryService!.DeletePolicy(_selectedPolicy!.PolicyId);
        _policies?.RemoveAll(p => p.PolicyId.Equals(_selectedPolicy!.PolicyId, StringComparison.OrdinalIgnoreCase));
        _selectedPolicy = null;
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

    private void ResetEmployee()
    {
        _employee = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
            OrganizationId = EditedOrganization!.Identifier,
            Organization = EditedOrganization
        };
    }

    private void AddEmployee()
    {
        EditedOrganization!.Employees.Add(_employee);
        ResetEmployee();
    }

    private void DeleteEmployee()
    {
        EditedOrganization!.Employees.Remove(EditedOrganization.Employees.First(e => e.EmployeeId.Equals(_employee.EmployeeId, StringComparison.OrdinalIgnoreCase)));
        ResetEmployee();
    }

    private void ResetOrganizationProperty()
    {
        _organizationProperty = new(string.Empty, string.Empty);
    }

    private void AddOrganizationProperty()
    {
        EditedOrganization!.Properties.Add(_organizationProperty);
        ResetOrganizationProperty();
    }

    private void DeleteOrganizationProperty()
    {
        EditedOrganization!.Properties.Remove(EditedOrganization.Properties.First(p => p.Key.Equals(_organizationProperty.Key, StringComparison.OrdinalIgnoreCase)));
        ResetOrganizationProperty();
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

    private void ResetPolicyProperty()
    {
        _policyProperty = new(string.Empty, string.Empty);
    }

    private void AddPolicyProperty()
    {
        EditedPolicy!.Properties.Add(_policyProperty);
        ResetPolicyProperty();
    }

    private void DeletePolicyProperty()
    {
        EditedPolicy!.Properties.Remove(EditedPolicy.Properties.First(p => p.Key.Equals(_policyProperty.Key, StringComparison.OrdinalIgnoreCase)));
        ResetPolicyProperty();
    }
}