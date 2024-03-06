namespace Commerce.Domain;

public static class DomainErrors
{
    public static class User
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.User)}.{nameof(Domain.User.Create)}";

            public const string FirstNameNullOrEmpty = $"{Prefix}.{nameof(FirstNameNullOrEmpty)}";
            public const string LastNameNullOrEmpty = $"{Prefix}.{nameof(LastNameNullOrEmpty)}";
            public const string EmailAddressNullOrEmpty = $"{Prefix}.{nameof(EmailAddressNullOrEmpty)}";

            public const string FirstNameLongerThanMaxLength = $"{Prefix}.{nameof(FirstNameLongerThanMaxLength)}";
            public const string LastNameLongerThanMaxLength = $"{Prefix}.{nameof(LastNameLongerThanMaxLength)}";
            public const string EmailAddressLongerThanMaxLength = $"{Prefix}.{nameof(EmailAddressLongerThanMaxLength)}";

            public const string EmailAddressInvalidFormat = $"{Prefix}.{nameof(EmailAddressInvalidFormat)}";
        }

        public static class ChangeDetails
        {
            private const string Prefix = $"{nameof(Domain.User)}.{nameof(Domain.User.ChangeDetails)}";

            public const string FirstNameNullOrEmpty = $"{Prefix}.{nameof(FirstNameNullOrEmpty)}";
            public const string LastNameNullOrEmpty = $"{Prefix}.{nameof(LastNameNullOrEmpty)}";
            public const string EmailAddressNullOrEmpty = $"{Prefix}.{nameof(EmailAddressNullOrEmpty)}";

            public const string FirstNameLongerThanMaxLength = $"{Prefix}.{nameof(FirstNameLongerThanMaxLength)}";
            public const string LastNameLongerThanMaxLength = $"{Prefix}.{nameof(LastNameLongerThanMaxLength)}";
            public const string EmailAddressLongerThanMaxLength = $"{Prefix}.{nameof(EmailAddressLongerThanMaxLength)}";

            public const string EmailAddressInvalidFormat = $"{Prefix}.{nameof(EmailAddressInvalidFormat)}";
        }
    }

    public static class MediaAsset
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.MediaAsset)}.{nameof(Domain.MediaAsset.Create)}";

            public const string BadId = $"{Prefix}.{nameof(BadId)}";
            public const string BadTimestamp = $"{Prefix}.{nameof(BadTimestamp)}";
            public const string NullOrEmptyRelativePath = $"{Prefix}.{nameof(NullOrEmptyRelativePath)}";
            public const string NullOrEmptyAbsolutePath = $"{Prefix}.{nameof(NullOrEmptyAbsolutePath)}";
        }
    }

    public static class UserGate
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.UserGate)}.{nameof(Domain.UserGate.Create)}";

            public const string UserIdEmpty = $"{Prefix}.{nameof(UserIdEmpty)}";
        }

        public static class Pass
        {
            private const string Prefix = $"{nameof(Domain.UserGate)}.{nameof(Domain.UserGate.Pass)}";

            public const string PassCodeInvalid = $"{Prefix}.{nameof(PassCodeInvalid)}";
            public const string UserGateAlreadyPassed = $"{Prefix}.{nameof(UserGateAlreadyPassed)}";
            public const string UserGateExpired = $"{Prefix}.{nameof(UserGateExpired)}";
        }

        public static class Exchange
        {
            private const string Prefix = $"{nameof(Domain.UserGate)}.{nameof(Domain.UserGate.Exchange)}";

            public const string UserGateNotPassed = $"{Prefix}.{nameof(UserGateNotPassed)}";
            public const string UserGateAlreadyExchanged = $"{Prefix}.{nameof(UserGateAlreadyExchanged)}";
            public const string UserGateExpired = $"{Prefix}.{nameof(UserGateExpired)}";
            public const string SecretInvalid = $"{Prefix}.{nameof(SecretInvalid)}";
        }
    }

    public static class Category
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.Category)}.{nameof(Domain.Category.Create)}";

            public const string NameNullOrEmpty = $"{Prefix}.{nameof(NameNullOrEmpty)}";
            public const string DescriptionNullOrEmpty = $"{Prefix}.{nameof(DescriptionNullOrEmpty)}";

            public const string NameLongerThanMaxLength = $"{Prefix}.{nameof(NameLongerThanMaxLength)}";
            public const string DescriptionLongerThanMaxLength = $"{Prefix}.{nameof(DescriptionLongerThanMaxLength)}";

            public const string NameShorterThanMinLength = $"{Prefix}.{nameof(NameShorterThanMinLength)}";
            public const string DescriptionShorterThanMinLength = $"{Prefix}.{nameof(DescriptionShorterThanMinLength)}";
        }
    }

    public static class Product
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.Create)}";

            public const string NameNullOrEmpty = $"{Prefix}.{nameof(NameNullOrEmpty)}";
            public const string NameShorterThanMinLength = $"{Prefix}.{nameof(NameShorterThanMinLength)}";
            public const string NameLongerThanMaxLength = $"{Prefix}.{nameof(NameLongerThanMaxLength)}";

            public const string DescriptionNullOrEmpty = $"{Prefix}.{nameof(DescriptionNullOrEmpty)}";
            public const string DescriptionShorterThanMinLength = $"{Prefix}.{nameof(DescriptionShorterThanMinLength)}";
            public const string DescriptionLongerThanMaxLength = $"{Prefix}.{nameof(DescriptionLongerThanMaxLength)}";

            public const string PriceLessOrEqualToZero = $"{Prefix}.{nameof(PriceLessOrEqualToZero)}";

            public const string CategoryIdEmpty = $"{Prefix}.{nameof(CategoryIdEmpty)}";
        }

        public static class ChangeCategory
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.ChangeCategory)}";

            public const string CategoryIdEmpty = $"{Prefix}.{nameof(CategoryIdEmpty)}";
        }

        public static class ChangeDetails
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.ChangeDetails)}";

            public const string NameNullOrEmpty = $"{Prefix}{nameof(NameNullOrEmpty)}";
            public const string NameLongerThanMaxLength = $"{Prefix}{nameof(NameLongerThanMaxLength)}";
            public const string NameShorterThanMinLength = $"{Prefix}{nameof(NameShorterThanMinLength)}";

            public const string DescriptionNullOrEmpty = $"{Prefix}{nameof(DescriptionNullOrEmpty)}";
            public const string DescriptionLongerThanMaxLength = $"{Prefix}{nameof(DescriptionLongerThanMaxLength)}";
            public const string DescriptionShorterThanMinLength = $"{Prefix}{nameof(DescriptionShorterThanMinLength)}";

            public const string PriceLessOrEqualToZero = $"{Prefix}{nameof(PriceLessOrEqualToZero)}";
        }
      
        public static class MakeVisible
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.MakeVisible)}";

            public const string ProductAlreadyVisible = $"{Prefix}.{nameof(ProductAlreadyVisible)}";
        }

        public static class MakeInvisible
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.MakeInvisible)}";

            public const string ProductAlreadyInvisible = $"{Prefix}.{nameof(ProductAlreadyInvisible)}";
        }

        public static class ChangeImage
        {
            private const string Prefix = $"{nameof(Domain.Product)}.{nameof(Domain.Product.ChangeImage)}";

            public const string MediaAssetNull = $"{Prefix}.{nameof(MediaAssetNull)}";
        }
    }

    public static class FavoriteProductSnapshot
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.FavoriteProductSnapshot)}.{nameof(Domain.FavoriteProductSnapshot.Create)}";

            public const string UserIdEmpty = $"{Prefix}.{nameof(UserIdEmpty)}";
            public const string ProductNull = $"{Prefix}.{nameof(ProductNull)}";
        }
    }
  
    public static class Order
    {
        public static class Create
        {
            private const string Prefix = $"{nameof(Domain.Order)}.{nameof(Domain.Order.Create)}";

            public const string ProductsEmpty = $"{Prefix}.{nameof(ProductsEmpty)}";
            public const string ShippingAddressNullOrEmpty = $"{Prefix}.{nameof(ShippingAddressNullOrEmpty)}";
        }
    }

    public static class OrderedProduct
    {
        public static class Confirm
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Confirm)}";

            public const string NotPending = $"{Prefix}.{nameof(NotPending)}";
        }

        public static class Process
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Process)}";

            public const string NotConfirmed = $"{Prefix}.{nameof(NotConfirmed)}";
        }

        public static class Deliver
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Deliver)}";

            public const string NotProcessing = $"{Prefix}.{nameof(NotProcessing)}";
        }

        public static class Fulfill
        {
            private const string Prefix = $"{nameof(Domain.OrderedProduct)}.{nameof(Domain.OrderedProduct.Fulfill)}";

            public const string NotInDelivery = $"{Prefix}.{nameof(NotInDelivery)}";
        }
    }
}