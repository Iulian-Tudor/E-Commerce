namespace Commerce.Client;

public interface IUserGateService
{
    Task<ApiResult<CreateUserGateResponse>> Create(CreateUserGateModel model);

    Task<ApiResult<PassUserGateResponse>> Pass(PassUserGateModel model);

    Task<ApiResult<ExchangeUserGateResponse>> Exchange(ExchangeUserGateModel model);
}

public sealed class UserGateService(IHttpClient http) : IUserGateService
{
    public async Task<ApiResult<CreateUserGateResponse>> Create(CreateUserGateModel model) => await http.Post<CreateUserGateModel, CreateUserGateResponse>(Routes.Auth.CreateUserGate, model);

    public async Task<ApiResult<PassUserGateResponse>> Pass(PassUserGateModel model) => await http.Patch<PassUserGateModel, PassUserGateResponse>(Routes.Auth.PassUserGate, model);

    public async Task<ApiResult<ExchangeUserGateResponse>> Exchange(ExchangeUserGateModel model) => await http.Patch<ExchangeUserGateModel, ExchangeUserGateResponse>(Routes.Auth.ExchangeUserGate, model);
}