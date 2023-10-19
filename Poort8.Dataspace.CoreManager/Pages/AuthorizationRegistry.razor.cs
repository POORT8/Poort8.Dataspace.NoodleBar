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

    protected override async Task OnInitializedAsync()
    {
        _organizations = (await AuthorizationRegistryService!.ReadOrganizations()).ToList();
        _products = (await AuthorizationRegistryService.ReadProducts()).ToList();

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
}