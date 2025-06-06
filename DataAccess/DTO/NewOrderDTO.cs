namespace DataAccess.DTO;

public record NewOrderDTO(
    Guid CustomerId,
    List<NewOrderItemDto> OrderItems,
    string Street,
    string City,
    string Country,
    string PostCode,
    string PaymentProvider
);