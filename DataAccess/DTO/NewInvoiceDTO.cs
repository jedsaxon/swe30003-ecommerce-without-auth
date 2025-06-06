namespace DataAccess.DTO;

public record NewInvoiceDTO(
    Guid InvoiceId,
    Guid OrderId,
    string Status,
    Guid CustomerId
);
