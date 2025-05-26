namespace DataAccess.DTO;

public record NewInvoiceDTO(Guid OrderId, string Status, Guid CustomerId);