using Commerce.Domain;

namespace Commerce.Business;

public interface IProductRepository : IRepository<Product, ProductReadModel>;