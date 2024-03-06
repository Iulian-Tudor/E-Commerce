using MudBlazor;

namespace Commerce.Client.Components.ShoppingCart.Pages;

public sealed partial class ShoppingCart
{
    public Cart Cart { get; set; } = new([]);

    public List<ProductModel> Products { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo("/categories");
            return;
        }

        Cart = await ShoppingCartService.Get();

        if (Cart.Products.Any())
        {
            var products = await ProductService.GetMany(Cart.Products.Keys.ToList());
            Products = products.ToList();
        }
    }

    private async Task Add(Guid productId)
    {
        await ShoppingCartService.Add(productId);
        Cart.Products[productId]++;
    }

    private async Task Remove(Guid productId)
    {
        await ShoppingCartService.Remove(productId);
        Cart.Products[productId]--;

        if (Cart.Products[productId] == 0)
        {
            await Delete(productId);
            Products.RemoveAll(p => p.Id == productId);
        }
    }

    private async Task Delete(Guid productId)
    {
        await ShoppingCartService.Delete(productId);
        Cart.Products.Remove(productId);
        Products.RemoveAll(p => p.Id == productId);
    }

    private decimal GetTotal()
    {
        return Products.Sum(p => p.Price * Cart.Products[p.Id]);
    }

    private void OpenDialog()
    {
        var options = new DialogOptions
        {
            ClassBackground = "blur-bg-modal",
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true,
            CloseButton = true,
            CloseOnEscapeKey = false
        };
        DialogService.Show<PlaceOrderDialog>("Place your order", options);
    }
}