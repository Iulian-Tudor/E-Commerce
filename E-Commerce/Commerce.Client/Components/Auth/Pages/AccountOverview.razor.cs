using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Auth.Pages;

public sealed partial class AccountOverview
{
    private UserModel? user;
    private Result<IdentifiedUser> identifiedUserResult;
    private bool isViewing = true;

    [SupplyParameterFromForm]
    private ChangeUserDetailsModel? Model { get; set; } = new();

    private string? EmailAddress { get; set; }

    protected override async Task OnInitializedAsync()
    {
        identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo("/categories");
        }

        user = await UserService.Get(identifiedUserResult.Value.Id);
        if (user is null)
        {
            NavigationManager.NavigateTo("/categories");
        }

        ResetForm();
    }

    private void ResetForm(string? firstName = null, string? lastName = null, string? emailAddress = null)
    {
        Model = new()
        {
            FirstName = firstName ?? user.FirstName,
            LastName = lastName ?? user.LastName
        };
        EmailAddress = emailAddress ?? user.EmailAddress;

        user.FirstName = firstName ?? user.FirstName;
        user.LastName = lastName ?? user.LastName;

        isViewing = true;
    }

    private async Task Submit()
    {
        if (identifiedUserResult.IsFailure)
        {
            return;
        }

        var response = await UserService.ChangeDetails(identifiedUserResult.Value.Id, Model);
        if (response.IsSuccess)
        {
            ResetForm(Model.FirstName, Model.LastName, EmailAddress);
        }
    }
}