using FluentValidation;
using Cropper.Blazor.Models;
using Cropper.Blazor.Components;
using Cropper.Blazor.Extensions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Commerce.Client.Components.Product.Pages;

public sealed partial class ChangeProductDetails
{
    private ProductModel? product = new();
    private Result<IdentifiedUser> identifiedUserResult;

    private ChangeProductImageModel changeImageModel = new();
    private ModelFluentValidator validationRules = new();
    private bool isValid;
    private bool isTouched;
    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
    private string _dragClass = DefaultDragClass;

    private CropperComponent? cropperComponent = null!;
    private readonly Options cropperOptions = new()
    {
        AspectRatio = 1,
        Background = false
    };

    [Parameter]
    public string CategoryId { get; set; }

    [Parameter]
    public string ProductId { get; set; }

    [SupplyParameterFromForm]
    private ChangeProductDetailsModel? ChangeDetailsModel { get; set; } = new();

    private bool IsErrorLoadImage { get; set; } = false;

    private bool IsAvailableInitCropper { get; set; } = true;

    public string Src { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo($"/categories/{CategoryId}");
        }

        product = await ProductService.Get(Guid.Parse(ProductId));
        if (product is null)
        {
            NavigationManager.NavigateTo($"/categories/{CategoryId}");
        }
        else
        {
            if (identifiedUserResult.Value.Id != product.VendorId)
            {
                NavigationManager.NavigateTo($"/categories/{CategoryId}");
            }
            ResetForm();
        }
    }

    private void ResetForm(string? name = null, string? description = null, decimal? price = null)
    {
        ChangeDetailsModel = new()
        {
            Name = name ?? product.Name,
            Description = description ?? product.Description,
            Price = price ?? product.Price
        };

        product.Name = name ?? product.Name;
        product.Description = description ?? product.Description;
        product.Price = price ?? product.Price;

    }

    private async Task SubmitChangeDetails()
    {
        if (identifiedUserResult.IsFailure)
        {
            return;
        }

        var response = await ProductService.ChangeDetails(product.Id, ChangeDetailsModel);
        if (response.IsSuccess)
        {
            ResetForm(ChangeDetailsModel.Name, ChangeDetailsModel.Description, ChangeDetailsModel.Price);
            NavigationManager.NavigateTo($"/categories/{CategoryId}/products");
        }
    }

    private async Task SubmitChangeImage()
    {
        if (identifiedUserResult.IsFailure)
        {
            return;
        }

        if (!changeImageModel.Files.Any())
        {
            return;
        }

        var imageOptions = new GetCroppedCanvasOptions
        {
            MaxHeight = 500,
            MaxWidth = 500,
            ImageSmoothingQuality = ImageSmoothingQuality.Medium.ToEnumString()
        };
        var imageBase64Data = await cropperComponent.GetCroppedCanvasDataURLAsync(imageOptions);
        var imageData = imageBase64Data.Split(',')[1];
        
        var fileAsStream = new MemoryStream(Convert.FromBase64String(imageData));
        var fileName = changeImageModel.Files.First().Name;
        var contentType = changeImageModel.Files.First().ContentType;
        var response = await ProductService.ChangeImage(product.Id, fileAsStream, fileName, contentType);

        if (response.IsSuccess)
        {
            changeImageModel.Files = new List<IBrowserFile>();
        }
    }

    private async Task ToggleVisibility()
    {
        if (!product.IsVisible)
        {
            var response = await ProductService.MakeVisible(product.Id);
            if (response.IsSuccess)
            {
                product.IsVisible = true;
            }
        }
        else
        {
            var response = await ProductService.MakeInvisible(product.Id);
            if (response.IsSuccess)
            {
                product.IsVisible = false;
            }
        }
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo($"/categories/{CategoryId}/products");
    }

    private void SetDragClass() => _dragClass = $"{DefaultDragClass} mud-border-primary";

    private void ClearDragClass() => _dragClass = DefaultDragClass;

    public class ModelFluentValidator : AbstractValidator<ChangeProductImageModel>
    {
        public ModelFluentValidator()
        {
            RuleFor(x => x.Files)
                .NotNull()
                .NotEmpty()
                .WithMessage("Please select a file");

            RuleFor(x => x.Files)
                .Must(x => x.All(f => f.Size <= 1024000))
                .WithMessage("File size must be less than 1MB");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ChangeProductImageModel>.CreateWithOptions((ChangeProductImageModel)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
    }

    public async Task RebuildImageAsync(InputFileChangeEventArgs inputFileChangeEventArgs)
    {
        var imageFile = inputFileChangeEventArgs.File;

        if (imageFile != null)
        {
            var oldSrc = Src;

            var fileStream = imageFile.OpenReadStream(imageFile.Size);
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            Src = $"data:image/png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";

            IsAvailableInitCropper = true;
            IsErrorLoadImage = false;

            cropperComponent?.Destroy();
            cropperComponent?.RevokeObjectUrlAsync(oldSrc);
        }
    }

    public void OnErrorLoadImageEvent(Microsoft.AspNetCore.Components.Web.ErrorEventArgs errorEventArgs)
    {
        Console.WriteLine(errorEventArgs.Message);
        IsErrorLoadImage = true;
        Destroy();
        StateHasChanged();
    }

    private void Destroy()
    {
        cropperComponent?.Destroy();
        cropperComponent?.RevokeObjectUrlAsync(Src);
    }
}