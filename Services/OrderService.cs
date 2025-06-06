using DataAccess.DTO;
using DataAccess.Repositories;

namespace Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly InvoiceService _invoiceService;

    public OrderService(IOrderRepository orderRepository, InvoiceService invoiceService)
    {
        _orderRepository = orderRepository;
        _invoiceService = invoiceService;
    }

    public async Task<bool> PlaceOrder(PlaceOrderDTO orderDto)
    {
        // Any business logic can go here (e.g., validation, logging, etc.)
        // For now, just pass through to the repository
        var orderItemsDto = orderDto.Items.Select(x => new NewOrderItemDto(
            x.ProductId,
            x.OrderId, // Ensure this is set correctly, or use Guid.Empty if not available
            x.Name,
            x.ShortDescription,
            x.QuantityOrdered,
            x.PricePaid
        )).ToList();
        var newOrderDto = new NewOrderDTO(
            Guid.NewGuid(), // Generate a new ID for the order
            orderDto.CustomerId,
            orderDto.Items,
            orderDto.Street,
            orderDto.City,
            orderDto.Country,
            orderDto.PostCode,
            orderDto.PaymentProvider
        );

        // Add the order and get the created order
        var createdOrder = await _orderRepository.AddOrder(newOrderDto);
        Console.WriteLine($"[DEBUG] Created order with ID: {createdOrder.OrderId}");

        // Create an invoice for the order
        var invoice = new NewInvoiceDTO(
            Guid.NewGuid(),
            createdOrder.OrderId,
            "PAID",
            orderDto.CustomerId
        );
        Console.WriteLine($"[DEBUG] Creating invoice for order ID: {invoice.OrderId}, invoice ID: {invoice.InvoiceId}");
        await _invoiceService.CreateInvoice(invoice);

        return true;
    }
} 
