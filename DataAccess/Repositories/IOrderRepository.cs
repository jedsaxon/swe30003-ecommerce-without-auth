using DataAccess.DTO;

namespace DataAccess.Repositories;

public interface IOrderRepository
{
    Task<List<OrderDTO>> GetOrders();
    Task<OrderDTO?> GetOrder(Guid orderId);
    Task<OrderDTO> AddOrder(NewOrderDTO e);
    Task UpdateOrder(OrderDTO e);
}