namespace DataAccess.DTO;

public record NewOrderItemDto(Guid ProductId, Guid OrderId, string Name, string ShortDescription, int QuantityOrdered);