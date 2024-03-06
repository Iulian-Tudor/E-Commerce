namespace Commerce.Infrastructure;

public class InfrastructureErrors
{
    public static class SendGridEmailService
    {
        public static class Send
        {
            private const string Prefix = $"{nameof(Infrastructure)}.{nameof(Infrastructure.SendGridEmailService)}.{nameof(Infrastructure.SendGridEmailService.Send)}";

            public const string SendFailed = $"{Prefix}.{nameof(SendFailed)}";
        }
    }

    public static class JwtTokenService
    {
        public static class GenerateToken
        {
            private const string Prefix = $"{nameof(Infrastructure)}.{nameof(Infrastructure.JwtTokenService)}.{nameof(GenerateToken)}";

            public const string UserNotProvided = $"{Prefix}.{nameof(UserNotProvided)}";
        }
    }

    public static class AzureBlobStorage
    {
        public static class Upload
        {
            private const string Prefix = $"{nameof(Infrastructure)}.{nameof(Infrastructure.AzureBlobStorage)}.{nameof(Upload)}";

            public const string BlobNull = $"{Prefix}.{nameof(BlobNull)}";
            public const string BlobNameNullOrEmpty = $"{Prefix}.{nameof(BlobNameNullOrEmpty)}";
        }
    }
}