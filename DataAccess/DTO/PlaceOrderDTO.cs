namespace DataAccess.DTO;

using System;
using System.Collections.Generic;

public class PlaceOrderDTO
{
    public Guid CustomerId { get; set; }
    public List<NewOrderItemDto> Items { get; set; } = new();
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public string PaymentProvider { get; set; } = string.Empty;
} 