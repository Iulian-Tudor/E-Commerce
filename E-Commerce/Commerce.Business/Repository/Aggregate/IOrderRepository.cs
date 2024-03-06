using Commerce.Domain;

namespace Commerce.Business;

public interface IOrderRepository : IRepository<Order, OrderReadModel>;