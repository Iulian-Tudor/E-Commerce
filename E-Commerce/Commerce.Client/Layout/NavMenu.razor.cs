using CSharpFunctionalExtensions;

namespace Commerce.Client.Layout;

public sealed partial class NavMenu
{
    private Result<IdentifiedUser> identifiedUserResult;

    protected override async Task OnInitializedAsync()
    {
        identifiedUserResult = await AuthService.GetIdentifiedUser();
    }
}