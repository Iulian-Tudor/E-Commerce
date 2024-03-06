using Blazored.LocalStorage;
using Blazored.Toast.Services;

namespace Commerce.Client;

public interface IShoppingCartService
{
    Task<Cart> Get();

    Task<int> Count();

    Task Add(Guid productId);

    Task Delete(Guid productId);

    Task Remove(Guid productId);

    Task Clear(bool showMessage = true);
}

public sealed class ShoppingCartService(ILocalStorageService localStorageService, IToastService toastService) : IShoppingCartService
{
    public async Task<Cart> Get()
    {
        var cart = await localStorageService.GetItemAsync<Cart>("cart");
        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart([]);
        await localStorageService.SetItemAsync("cart", cart);

        return cart;
    }

    public async Task<int> Count()
    {
        var cart = await Get();
        return cart.Products.Sum(p => p.Value);
    }

    public async Task Add(Guid productId)
    {
        var cart = await Get();

        if (!cart.Products.TryAdd(productId, 1))
        {
            cart.Products[productId]++;
        }

        await localStorageService.SetItemAsync("cart", cart);

        toastService.ShowSuccess("Product added to cart.");
    }

    public async Task Delete(Guid productId)
    {
        var cart = await Get();

        if (cart.Products.ContainsKey(productId))
        {
            cart.Products.Remove(productId);
        }

        await localStorageService.SetItemAsync("cart", cart);

        toastService.ShowSuccess("Product removed from cart.");
    }

    public async Task Remove(Guid productId)
    {
        var cart = await Get();

        if (cart.Products.ContainsKey(productId))
        {
            cart.Products[productId]--;
        }

        if (cart.Products[productId] <= 0)
        {
            cart.Products.Remove(productId);
        }

        await localStorageService.SetItemAsync("cart", cart);

        toastService.ShowSuccess("Product removed from cart.");
    }

    public async Task Clear(bool showMessage = true)
    {
        await localStorageService.RemoveItemAsync("cart");

        if (showMessage)
        {
	        toastService.ShowSuccess("Cart cleared.");
        }
    }
}