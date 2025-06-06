namespace DataAccess.DTO;

public record OrderDTO(
    Guid OrderId,
    Guid CustomerId,
    List<OrderItem> Items
);
