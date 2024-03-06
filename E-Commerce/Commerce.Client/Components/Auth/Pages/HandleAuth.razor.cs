using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Auth.Pages;

public sealed partial class HandleAuth
{
    private bool isCreated;
    private bool isPassed;

    [SupplyParameterFromForm]
    public CreateUserGateModel? CreateModel { get; set; } = new();

    [SupplyParameterFromForm]
    public PassUserGateModel? PassModel { get; set; } = new();

    private async Task SubmitCreate()
    {
        var response = await UserGateService.Create(CreateModel);
        if (response.IsFailure)
        {
            return;
        }

        isCreated = true;
        PassModel!.UserId = response.Value.UserId;
    }

    private async Task SubmitPass()
    {
        var response = await UserGateService.Pass(PassModel);
        if (response.IsFailure)
        {
            return;
        }

        isPassed = true;
        var exchangeModel = new ExchangeUserGateModel
        {
            UserId = PassModel.UserId,
            GateSecret = response.Value.GateSecret
        };

        var exchangeResponse = await UserGateService.Exchange(exchangeModel);
        if (exchangeResponse.IsFailure)
        {
            return;
        }

        await LocalStorageService.SetItemAsync("authToken", exchangeResponse.Value.AuthToken);
        await LocalStorageService.SetItemAsync("refreshToken", exchangeResponse.Value.RefreshToken.Value);

        NavigationManager.NavigateTo("/categories", true);
    }
}