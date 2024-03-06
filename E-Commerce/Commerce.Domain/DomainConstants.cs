namespace Commerce.Domain;

public static class DomainConstants
{
    public static class User
    {
        public const int FirstNameMaxLength = 50;
        public const int LastNameMaxLength = 50;
        public const int EmailAddressMaxLength = 50;
    }

    public static class UserGate
    {
        public const int LifetimeInMinutes = 10;
        public const int ExchangeWindow = 1;
    }

    public static class Category
    {
        public const int NameMaxLength = 25;
        public const int NameMinLength = 3;
        public const int DescriptionMaxLength = 100;
        public const int DescriptionMinLength = 10;
    }

    public static class Product
    {
        public const int NameMaxLength = 50;
        public const int NameMinLength = 3;
        public const int DescriptionMaxLength = 400;
        public const int DescriptionMinLength = 10;
    }
}