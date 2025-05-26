namespace DataAccess.DTO;

public record OrderItem(
    Guid Id,
    Guid ProductId,
    Guid OrderId,
    double PricePaid,
    string Name,
    string ShortDescription,
    int QuantityOrdered);