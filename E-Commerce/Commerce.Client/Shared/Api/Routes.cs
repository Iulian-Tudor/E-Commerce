namespace Commerce.Client;

public static class Routes
{
    public const string Base = "http://localhost:7373/api/v1";

    public static class Auth
    {
        public const string CreateUserGate = $"{Base}/user-gates";
        public const string PassUserGate = $"{Base}/user-gates/passage";
        public const string ExchangeUserGate = $"{Base}/user-gates/exchange";
        public const string RefreshToken = $"{Base}/token/refresh";
    }

    public static class Users
    {
        private const string Prefix = $"{Base}/users";

        public const string Create = Prefix;
        public const string GetAll = Prefix;
        public const string GetById = $"{Prefix}/{{id}}";
        public const string ChangeDetails = $"{Prefix}/{{id}}/details";
        public const string Delete = $"{Prefix}/{{id}}";
    }

    public static class Products
    {
        private const string Prefix = $"{Base}/products";

        public const string GetAll = Prefix;
        public const string GetById = $"{Prefix}/{{id}}";
        public const string Create = Prefix;
        public const string Delete = $"{Prefix}/{{id}}";
        public const string MakeVisible = $"{Prefix}/{{id}}/visibility";
        public const string MakeInvisible = $"{Prefix}/{{id}}/visibility";
        public const string ChangeDetails = $"{Prefix}/{{id}}/details";
        public const string ChangeImage = $"{Prefix}/{{id}}/images";
        public const string AddToFavorites = $"{Base}/favorite-products";
        public const string RemoveFromFavorites = $"{Base}/favorite-products/{{id}}";
        public const string GetFavorites = $"{Base}/favorite-products";
    }

    public static class Categories
    {
        private const string Prefix = $"{Base}/categories";

        public const string GetAll = Prefix;
        public const string GetById = $"{Prefix}/{{id}}";
        public const string Create = Prefix;
    }

    public static class Orders
    {
        private const string Prefix = $"{Base}/orders";

        public const string Place = Prefix;
    }

    public static class OrderedProducts
    {
        private const string Prefix = $"{Base}/orders";

        public const string Get = $"{Prefix}/{{id}}";
        public const string GetAll = $"{Prefix}";
        public const string GetMany = $"{Prefix}";
        public const string GetAllByVendor = $"{Prefix}";
        public const string Confirm = $"{Prefix}/{{orderId}}/products/{{orderedProductId}}/confirmation";
        public const string Process = $"{Prefix}/{{orderId}}/products/{{orderedProductId}}/process";
        public const string Deliver = $"{Prefix}/{{orderId}}/products/{{orderedProductId}}/delivery";
        public const string Fulfill = $"{Prefix}/{{orderId}}/products/{{orderedProductId}}/fulfillment";
    }
}