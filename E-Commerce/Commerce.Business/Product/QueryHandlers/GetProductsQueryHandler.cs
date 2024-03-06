using MediatR;

namespace Commerce.Business;

internal sealed class GetProductsQueryHandler(IProductRepository repository, IBlobStorage blobStorage) : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductAggregationModel>>
{
    public async Task<IReadOnlyCollection<ProductAggregationModel>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = repository.Query();

        if (request.Id.HasValue)
        {
            products = products.Where(p => p.Id == request.Id);
        }

        if (request.VendorId.HasValue)
        {
            products = products.Where(p => p.VendorId == request.VendorId);
        }

        if (request.CategoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == request.CategoryId);
        }

        if (request.Ids.Any())
        {
            products = products.Where(p => request.Ids.Contains(p.Id));
        }

        if (request.IsVisible.HasValue)
        {
			products = products.Where(p => p.IsVisible == request.IsVisible);
		}

        var aggregationModels = new List<ProductAggregationModel>();
        foreach (var product in products)
        {
            var aggregationModel = await MapToReadModel(product, blobStorage);
            aggregationModels.Add(aggregationModel);
        }

        return aggregationModels;
    }

    private static async Task<ProductAggregationModel> MapToReadModel(ProductReadModel product, IBlobStorage blobStorage)
    {
        var aggregationModel = new ProductAggregationModel
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            VendorId = product.VendorId,
            VendorName = product.VendorName,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            IsVisible = product.IsVisible
        };

        if (product.MediaAsset is null || product.MediaAsset == default || product.MediaAsset.Id == Guid.Empty)
        {
            return aggregationModel;
        }

        var image = await blobStorage.Get(product.MediaAsset.AbsolutePath);
        if (image.IsFailure)
        {
            return aggregationModel;
        }

        aggregationModel.Image = image.Value.Data;

        return aggregationModel;
    }
}

public sealed class ProductAggregationModel : ProductReadModel
{
    public byte[] Image { get; set; } = Array.Empty<byte>();
}