namespace Commerce.Business;

public static class BusinessErrors
{
    public static class User
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.User)}.{nameof(Domain.User.Create)}";

            public const string EmailAddressAlreadyInUse = $"{Prefix}.{nameof(EmailAddressAlreadyInUse)}";
        }

        public static class ChangeDetails
        {
            private const string Prefix = $"{nameof(Domain.User)}.{nameof(Domain.User.ChangeDetails)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
        }

        public static class Delete
        {
            private const string Prefix = $"{nameof(Domain.User)}.{nameof(Delete)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
        }
    }

    public static class UserGate
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.UserGate)}.{nameof(Domain.UserGate.Create)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
        }

        public static class Pass
        {
            private const string Prefix = $"{nameof(Domain.UserGate)}.{nameof(Domain.UserGate.Pass)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
            public const string UserGateNotFound = $"{Prefix}.{nameof(UserGateNotFound)}";
        }

        public static class Exchange
        {
            private const string Prefix = $"{nameof(Domain.UserGate)}.{nameof(Domain.UserGate.Exchange)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
            public const string UserGateNotFound = $"{Prefix}.{nameof(UserGateNotFound)}";
        }
    }

    public static class Category
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.Category)}.{nameof(Domain.Category.Create)}";

            public const string CategoryAlreadyExists = $"{Prefix}.{nameof(CategoryAlreadyExists)}";
        }
    }

    public static class Product
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.Create)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
        }

        public static class ChangeCategory
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.ChangeCategory)}";

            public const string CategoryNotFound = $"{Prefix}.{nameof(CategoryNotFound)}";
            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";

            public static string ProductDoesNotBelongToCaller = $"{Prefix}.{nameof(ProductDoesNotBelongToCaller)}";
        }
        
        public static class ChangeDetails
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.ChangeDetails)}";

            public const string ProductNotFound = $"{Prefix}{nameof(ProductNotFound)}";
            public const string ProductDoesNotBelongToCaller = $"{Prefix}.{nameof(ProductDoesNotBelongToCaller)}";
        }
      
        public static class Delete
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Delete)}";

            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";
            public const string ProductDoesNotBelongToCaller = $"{Prefix}.{nameof(ProductDoesNotBelongToCaller)}";
        }
      
        public static class MakeVisible
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.MakeVisible)}";

            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";
            public const string ProductDoesNotBelongToCaller = $"{Prefix}.{nameof(ProductDoesNotBelongToCaller)}";
        }

        public static class MakeInvisible
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.MakeInvisible)}";

            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";
            public const string ProductDoesNotBelongToCaller = $"{Prefix}.{nameof(ProductDoesNotBelongToCaller)}";
        }

        public static class UploadImage
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(UploadImage)}";

            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";
            public const string ProductDoesNotBelongToCaller = $"{Prefix}.{nameof(ProductDoesNotBelongToCaller)}";
            public const string ExtensionNotAccepted = $"{Prefix}.{nameof(ExtensionNotAccepted)}";
            public const string SizeExceedsMax = $"{Prefix}.{nameof(SizeExceedsMax)}";
        }
    }

    public static class FavoriteProductSnapshot
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.FavoriteProductSnapshot)}.{nameof(Domain.FavoriteProductSnapshot.Create)}";

            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";
            public const string ProductAlreadyInFavorites = $"{Prefix}.{nameof(ProductAlreadyInFavorites)}";
        }

        public static class Delete
        {
            private const string Prefix = $"{nameof(Domain.FavoriteProductSnapshot)}.{nameof(Delete)}";

            public const string FavoriteProductSnapshotNotFound = $"{Prefix}.{nameof(FavoriteProductSnapshotNotFound)}";
            public const string FavoriteProductSnapshotNotOwnedByCaller = $"{Prefix}.{nameof(FavoriteProductSnapshotNotOwnedByCaller)}";
        }
    }

    public static class Order
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.Order)}.{nameof(Domain.Order.Create)}";

            public const string UserNotFound = $"{Prefix}.{nameof(UserNotFound)}";
            public const string ProductNotFound = $"{Prefix}.{nameof(ProductNotFound)}";
        }
    }

    public static class OrderedProduct
    {
        public static class Confirm
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Confirm)}";

            public const string OrderNotFound = $"{Prefix}.{nameof(OrderNotFound)}";
            public const string OrderedProductNotFound = $"{Prefix}.{nameof(OrderedProductNotFound)}";
            public const string OrderedProductNotSoldByCaller = $"{Prefix}.{nameof(OrderedProductNotSoldByCaller)}";
        }

        public static class Process
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Process)}";

            public const string OrderNotFound = $"{Prefix}.{nameof(OrderNotFound)}";
            public const string OrderedProductNotFound = $"{Prefix}.{nameof(OrderedProductNotFound)}";
            public const string OrderedProductNotSoldByCaller = $"{Prefix}.{nameof(OrderedProductNotSoldByCaller)}";
        }

        public static class Deliver
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Deliver)}";

            public const string OrderNotFound = $"{Prefix}.{nameof(OrderNotFound)}";
            public const string OrderedProductNotFound = $"{Prefix}.{nameof(OrderedProductNotFound)}";
            public const string OrderedProductNotSoldByCaller = $"{Prefix}.{nameof(OrderedProductNotSoldByCaller)}";
        }

        public static class Fulfill
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Fulfill)}";

            public const string OrderNotFound = $"{Prefix}.{nameof(OrderNotFound)}";
            public const string OrderedProductNotFound = $"{Prefix}.{nameof(OrderedProductNotFound)}";
            public const string OrderedProductNotSoldByCaller = $"{Prefix}.{nameof(OrderedProductNotSoldByCaller)}";
        }
    }
}