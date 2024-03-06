namespace Commerce.Client.Components.Product.Pages;

public sealed partial class MyProductsOverview
{
	private IReadOnlyCollection<ProductModel> products = new List<ProductModel>();

	protected override async Task OnInitializedAsync()
	{
		var identifiedUserResult = await AuthService.GetIdentifiedUser();

		if (identifiedUserResult.IsFailure)
		{
			NavigationManager.NavigateTo("/categories");
			return;
		}

		products = await ProductService.GetAllByVendor(identifiedUserResult.Value.Id);
	}

	private void NavigateToEditProduct(ProductModel product)
	{
		NavigationManager.NavigateTo($"/categories/{product.CategoryId}/products/{product.Id}/edit");
	}

}