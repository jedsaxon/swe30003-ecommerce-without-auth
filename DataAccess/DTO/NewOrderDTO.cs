namespace DataAccess.DTO;

public record NewOrderDTO(Guid CustomerId, List<NewOrderItemDto> OrderItems);