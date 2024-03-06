namespace Commerce.Domain;

public static class UsersFactory
{
    public static User Any() => User.Create("First Name", "Last Name", "tudor.tescu@hotmail.com").Value;
}