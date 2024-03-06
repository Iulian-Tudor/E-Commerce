namespace Commerce.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommerceServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IHttpClient, HttpClientWrapper>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUserGateService, UserGateService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IOrderService, OrderService>()
            .AddScoped<IOrderedProductService, OrderedProductService>()
            .AddScoped<IShoppingCartService, ShoppingCartService>();
    }
}