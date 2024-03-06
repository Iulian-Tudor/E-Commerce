namespace Commerce.Client;

public interface IProductService
{
    Task<ProductModel?> Get(Guid id);

    Task<IReadOnlyCollection<ProductModel>> GetAll();

    Task<IReadOnlyCollection<ProductModel>> GetMany(List<Guid> ids);

    Task<IReadOnlyCollection<ProductModel>> GetAllByCategory(Guid categoryId);

    Task<IReadOnlyCollection<ProductModel>> GetAllByVendor(Guid vendorId);

    Task<ApiResult> Create(CreateProductModel model);

    Task<ApiResult> ChangeDetails(Guid id, ChangeProductDetailsModel model);

    Task<ApiResult> MakeVisible(Guid id);

    Task<ApiResult> MakeInvisible(Guid id);

    Task<ApiResult> ChangeImage(Guid id, Stream image, string fileName, string extension);

    Task<ApiResult> AddToFavorites(Guid id);

    Task<ApiResult> RemoveFromFavorites(Guid id);

    Task<IReadOnlyCollection<FavoriteProductModel>> GetFavorites();
}

public sealed class ProductService(IHttpClient http) : IProductService
{
    public async Task<ProductModel?> Get(Guid id) => await http.Get<ProductModel>(Routes.Products.GetById.WithRouteParams(new { Id = id }));

    public async Task<IReadOnlyCollection<ProductModel>> GetAll() 
	    => await http.Get<IReadOnlyCollection<ProductModel>>(Routes.Products.GetAll.WithQueryParams(new { IsVisible = true })) ?? new List<ProductModel>();

    public async Task<IReadOnlyCollection<ProductModel>> GetMany(List<Guid> ids) 
        => await http.Get<IReadOnlyCollection<ProductModel>>(Routes.Products.GetAll.WithQueryParams(new { ids })) ?? new List<ProductModel>();

    public async Task<IReadOnlyCollection<ProductModel>> GetAllByCategory(Guid categoryId) 
        => await http.Get<IReadOnlyCollection<ProductModel>>(Routes.Products.GetAll.WithQueryParams(new { categoryId, IsVisible = true })) ?? new List<ProductModel>();

    public async Task<IReadOnlyCollection<ProductModel>> GetAllByVendor(Guid vendorId) 
		=> await http.Get<IReadOnlyCollection<ProductModel>>(Routes.Products.GetAll.WithQueryParams(new { vendorId })) ?? new List<ProductModel>();

    public async Task<ApiResult> Create(CreateProductModel model) => await http.Post(Routes.Products.Create, model);

    public async Task<ApiResult> ChangeDetails(Guid id, ChangeProductDetailsModel model) => await http.Patch(Routes.Products.ChangeDetails.WithRouteParams(new { id }), model);

    public async Task<ApiResult> MakeVisible(Guid id) => await http.Patch(Routes.Products.MakeVisible.WithRouteParams(new { id }));

    public async Task<ApiResult> MakeInvisible(Guid id) => await http.Delete(Routes.Products.MakeInvisible.WithRouteParams(new { id }));

    public async Task<ApiResult> ChangeImage(Guid id, Stream image, string fileName, string extension) => await http.PostStream(Routes.Products.ChangeImage.WithRouteParams(new { id }), image, fileName, extension);
   
    public async Task<ApiResult> AddToFavorites(Guid id) => await http.Post(Routes.Products.AddToFavorites, new AddProductToFavoritesModel { ProductId = id });

    public async Task<ApiResult> RemoveFromFavorites(Guid id) => await http.Delete(Routes.Products.RemoveFromFavorites.WithRouteParams(new { id }));
    
    public async Task<IReadOnlyCollection<FavoriteProductModel>> GetFavorites() => await http.Get<IReadOnlyCollection<FavoriteProductModel>>(Routes.Products.GetFavorites);
}