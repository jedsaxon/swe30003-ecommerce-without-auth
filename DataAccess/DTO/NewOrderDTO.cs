namespace DataAccess.DTO;

public record NewOrderDTO(
    Guid Id,
    Guid CustomerId,
    List<NewOrderItemDto> OrderItems,
    string Street,
    string City,
    string Country,
    string PostCode,
    string PaymentProvider
);
