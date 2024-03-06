namespace Commerce.Client;

public interface IUserService
{
    Task<UserModel?> Get(Guid id);

    Task<ApiResult> Create(CreateUserModel model);

    Task<ApiResult> ChangeDetails(Guid userId, ChangeUserDetailsModel model);
}

public sealed class UserService(IHttpClient http) : IUserService
{
    public async Task<UserModel?> Get(Guid id) => await http.Get<UserModel>(Routes.Users.GetById.WithRouteParams(new { Id = id }));

    public async Task<ApiResult> Create(CreateUserModel model) => await http.Post(Routes.Users.Create, model);

    public async Task<ApiResult> ChangeDetails(Guid userId, ChangeUserDetailsModel model) => await http.Put(Routes.Users.ChangeDetails.WithRouteParams(new { Id = userId }), model);
}