namespace Commerce.Client;

public interface ICategoryService
{
    Task<CategoryModel?> Get(Guid id);

    Task<IReadOnlyCollection<CategoryModel>> GetAll();

    Task<ApiResult> Create(CreateCategoryModel model);
}

public sealed class CategoryService(IHttpClient http) : ICategoryService
{
    public async Task<CategoryModel?> Get(Guid id) => await http.Get<CategoryModel>(Routes.Categories.GetById.WithRouteParams(new { Id = id }));

    public async Task<IReadOnlyCollection<CategoryModel>> GetAll() => await http.Get<IReadOnlyCollection<CategoryModel>>(Routes.Categories.GetAll) ?? new List<CategoryModel>();

    public async Task<ApiResult> Create(CreateCategoryModel model) => await http.Post(Routes.Categories.Create, model);
}