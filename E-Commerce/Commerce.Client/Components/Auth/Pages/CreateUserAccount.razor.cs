using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Auth.Pages;

public sealed partial class CreateUserAccount
{
    [SupplyParameterFromForm]
    public CreateUserModel? Model { get; set; } = new();

    private async Task Submit()
    {
        var response = await UserService.Create(Model);
        if (response.IsSuccess)
        {
            NavigationManager.NavigateTo("sign-in");
        }
    }
}