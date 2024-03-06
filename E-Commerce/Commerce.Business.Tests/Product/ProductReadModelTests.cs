using Xunit;
using Commerce.Domain;
using FluentAssertions;


namespace Commerce.Business.Tests;
public sealed class ProductReadModelTests
{
	[Fact]
	public void Given_ToAggregate_Then_ShouldReturnCorrectProduct()
	{
		// Arrange
		var id = Guid.NewGuid();
		var categoryId = Guid.NewGuid();
		var vendorId = Guid.NewGuid();
		var name = "name";
		var description = "description";
		var price = new FixedPrecisionPrice(100m);
		var isVisible = true;
		var mediaAsset = MediaAsset.Create(Guid.NewGuid(), "relativePath", "absolutePath", DateTime.Now).Value;
		var readModel = new ProductReadModel
		{ 
			Id = id,
			CategoryId = categoryId,
			VendorId = vendorId,
			VendorName = "Test Vendor",
			Name = name,
			Description = description,
			Price = price,
			IsVisible = isVisible,
			MediaAsset = mediaAsset
		};

		// Act
		var product = readModel.ToAggregate();

		// Assert
		product.Id.Should().Be(id);
		product.CategoryId.Should().Be(categoryId);
		product.VendorId.Should().Be(vendorId);
		product.VendorName.Should().Be("Test Vendor");
		product.Name.Should().Be(name);
		product.Description.Should().Be(description);
		product.Price.Should().Be(price);
		product.IsVisible.Should().Be(isVisible);
		product.MediaAsset.Should().Be(mediaAsset);
	}

	[Fact]
	public void Given_FromAggregate_Then_ShouldReturnCorrectReadModel()
	{
		// Arrange
		var id = Guid.NewGuid();
		var categoryId = Guid.NewGuid();
		var vendorId = Guid.NewGuid();
		var name = "name";
		var description = "description";
		var price = new FixedPrecisionPrice(100m);
		var isVisible = true;
		var mediaAsset = MediaAsset.Create(Guid.NewGuid(), "relativePath", "absolutePath", DateTime.Now).Value;
		var product = Product.Create(id, vendorId, "Test Vendor", name, description, price, categoryId, isVisible, mediaAsset).Value;

		// Act
		var readModel = new ProductReadModel().FromAggregate(product);

		// Assert
		readModel.Id.Should().Be(id);
		readModel.CategoryId.Should().Be(categoryId);
		readModel.VendorId.Should().Be(vendorId);
		readModel.VendorName.Should().Be("Test Vendor");
		readModel.Name.Should().Be(name);
		readModel.Description.Should().Be(description);
		readModel.Price.Should().Be(price);
		readModel.IsVisible.Should().Be(isVisible);
		readModel.MediaAsset.Should().Be(mediaAsset);
	}
}

