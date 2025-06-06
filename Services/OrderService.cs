using DataAccess.DTO;
using DataAccess.Repositories;
using System.Threading.Tasks;

namespace Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> PlaceOrder(PlaceOrderDTO orderDto)
    {
        // Any business logic can go here (e.g., validation, logging, etc.)
        // For now, just pass through to the repository
        var orderItemsDto = orderDto.Items;
        var newOrderDto = new NewOrderDTO(
            orderDto.CustomerId,
            orderItemsDto,
            orderDto.Street,
            orderDto.City,
            orderDto.Country,
            orderDto.PostCode,
            orderDto.PaymentProvider
        );
        await _orderRepository.AddOrder(newOrderDto);
        return true;
    }
} 