using Microsoft.AspNetCore.Components.Forms;

namespace Commerce.Client;

public sealed class ChangeProductImageModel
{
    public string ImageData { get; set; } = string.Empty;

    public IReadOnlyList<IBrowserFile> Files { get; set; }

    public async Task SetImageData()
    {
        var fileAsStream = Files.First().OpenReadStream(1024000);
        using var memoryStream = new MemoryStream();
        await fileAsStream.CopyToAsync(memoryStream);

        ImageData =  $"data:image/png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
    }
}