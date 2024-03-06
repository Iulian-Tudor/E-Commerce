namespace Commerce.Business;

public static class BusinessConstants
{
    public static class Product
    {
        public static class ImageUpload
        {
            public const int MaxFileSize = 1 * 1024 * 1024; // 1 MB
            public static string[] AllowedExtensions = { "jpg", "jpeg", "png", "webp" };

            public static class Metadata
            {
                public const string ImageId = nameof(ImageId);
                public const string ProductId = nameof(ProductId);
                public const string MimeType = nameof(MimeType);
                public const string OriginalFileName = nameof(OriginalFileName);
                public const string SourceApp = nameof(SourceApp);
                public const string Action = nameof(Action);
            }
        }
    }
}