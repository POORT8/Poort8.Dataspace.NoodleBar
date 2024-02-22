using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Poort8.Dataspace.AuthorizationRegistry;
using Poort8.Dataspace.AuthorizationRegistry.Entities;
using Poort8.Dataspace.CoreManager.ARProducts.Dialogs;
using Poort8.Dataspace.CoreManager.Services;

namespace Poort8.Dataspace.CoreManager.ARProducts;

public partial class Index : IDisposable
{
    private bool disposedValue;
    [Inject]
    public required StateContainer StateContainer { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IAuthorizationRegistry AuthorizationRegistry { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }

    public IQueryable<Product>? Products;

    protected override async Task OnInitializedAsync()
    {
        StateContainer.OnChange += StateHasChanged;

        Products = (await AuthorizationRegistry!.ReadProducts()).AsQueryable();
    }

    private async Task AddNewClicked()
    {
        var parameters = new DialogParameters()
        {
            Title = $"New Product",
            PrimaryAction = "Add New Product",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "400px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleAddNewClicked)
        };

        var product = new Product("", "", "", "", "");
        await DialogService.ShowDialogAsync<ProductDialog>(product, parameters);
    }

    private async Task HandleAddNewClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.CreateProduct((Product)result.Data);
            Products = (await AuthorizationRegistry.ReadProducts()).AsQueryable();
        }
    }

    private void DetailsClicked(Product product)
    {
        StateContainer.CurrentProduct = product;
        NavigationManager.NavigateTo($"/ar/products/details/{product.ProductId}");
    }

    private async Task PropertiesClicked(Product product)
    {
        var parameters = new DialogParameters()
        {
            Title = $"Product Properties",
            PrimaryAction = "Save Properties",
            PrimaryActionEnabled = true,
            SecondaryAction = "Cancel",
            Width = "600px",
            TrapFocus = true,
            Modal = true,
            ShowDismiss = false,
            OnDialogResult = DialogService.CreateDialogCallback(this, HandleSavePropertiesClicked)
        };

        await DialogService.ShowDialogAsync<ProductPropertiesDialog>(product, parameters);
    }

    private async Task HandleSavePropertiesClicked(DialogResult result)
    {
        if (!result.Cancelled && result.Data is not null)
        {
            await AuthorizationRegistry.UpdateProduct((Product)result.Data);
            Products = (await AuthorizationRegistry.ReadProducts()).AsQueryable();
        }
    }

    private async Task DeleteClicked(Product product)
    {
        var dialog = await DialogService.ShowConfirmationAsync(
            $"Are you sure you want to delete {product.Name}?",
            "Delete",
            "Cancel",
            "Delete Product");

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await AuthorizationRegistry.DeleteProduct(product.ProductId);
            Products = (await AuthorizationRegistry.ReadProducts()).AsQueryable();
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
