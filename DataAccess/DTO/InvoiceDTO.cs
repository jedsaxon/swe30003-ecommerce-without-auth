using System;

namespace DataAccess.DTO;

public record InvoiceDTO(
    Guid InvoiceId,
    Guid OrderId,
    string Status,
    Guid CustomerId
);