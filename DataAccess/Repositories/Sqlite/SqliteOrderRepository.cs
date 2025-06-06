using DataAccess.DTO;

namespace DataAccess.Repositories.Sqlite;

public class SqliteOrderRepository : IOrderRepository
{
    private readonly SqliteDataAccess _dataAccess;

    public SqliteOrderRepository(SqliteDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    public async Task<List<OrderDTO>> GetOrders()
    {
        var orders = new List<OrderDTO>();
        using var command = await _dataAccess.CreateCommand();
        command.CommandText = @"
            SELECT o.id, o.customer_id, oi.id, oi.product_id, oi.order_id, oi.price_paid, oi.name, oi.short_description, oi.quantity_ordered
            FROM orders o
            LEFT JOIN order_items oi ON o.id = oi.order_id
            ORDER BY o.id";

        using var reader = await command.ExecuteReaderAsync();
        var currentOrderId = Guid.Empty;
        var currentOrderItems = new List<OrderItem>();
        var currentCustomerId = Guid.Empty;

        while (await reader.ReadAsync())
        {
            var orderId = Guid.Parse(reader.GetString(0));
            var customerId = Guid.Parse(reader.GetString(1));

            if (orderId != currentOrderId)
            {
                if (currentOrderId != Guid.Empty)
                {
                    orders.Add(new OrderDTO(currentOrderId, currentCustomerId, currentOrderItems));
                    currentOrderItems = new List<OrderItem>();
                }
                currentOrderId = orderId;
                currentCustomerId = customerId;
            }

            if (!reader.IsDBNull(2)) // Check if there are order items
            {
                currentOrderItems.Add(new OrderItem(
                    Guid.Parse(reader.GetString(2)), // Id
                    Guid.Parse(reader.GetString(3)), // ProductId
                    Guid.Parse(reader.GetString(4)), // OrderId
                    reader.GetDouble(5), // PricePaid
                    reader.GetString(6), // Name
                    reader.GetString(7), // ShortDescription
                    reader.GetInt32(8) // QuantityOrdered
                ));
            }
        }

        if (currentOrderId != Guid.Empty)
        {
            orders.Add(new OrderDTO(currentOrderId, currentCustomerId, currentOrderItems));
        }

        return orders;
    }

    public async Task<OrderDTO?> GetOrder(Guid orderId)
    {
        using var command = await _dataAccess.CreateCommand();
        command.CommandText = @"
            SELECT o.id, o.customer_id, oi.id, oi.product_id, oi.order_id, oi.price_paid, oi.name, oi.short_description, oi.quantity_ordered
            FROM orders o
            LEFT JOIN order_items oi ON o.id = oi.order_id
            WHERE o.id = @OrderId";
        command.Parameters.AddWithValue("@OrderId", orderId.ToString().ToLowerInvariant());

        using var reader = await command.ExecuteReaderAsync();
        var orderItems = new List<OrderItem>();
        Guid? customerId = null;

        while (await reader.ReadAsync())
        {
            customerId ??= Guid.Parse(reader.GetString(1));
            if (!reader.IsDBNull(2)) // Check if there are order items
            {
                orderItems.Add(new OrderItem(
                    Guid.Parse(reader.GetString(2)), // Id
                    Guid.Parse(reader.GetString(3)), // ProductId
                    Guid.Parse(reader.GetString(4)), // OrderId
                    reader.GetDouble(5), // PricePaid
                    reader.GetString(6), // Name
                    reader.GetString(7), // ShortDescription
                    reader.GetInt32(8) // QuantityOrdered
                ));
            }
        }

        return customerId.HasValue ? new OrderDTO(orderId, customerId.Value, orderItems) : null;
    }

    public async Task<OrderDTO> AddOrder(NewOrderDTO order)
    {
        using var command = await _dataAccess.CreateCommand();
        var orderId = Guid.NewGuid();
        command.CommandText = @"
            INSERT INTO orders (id, customer_id, street, city, country, post_code, payment_provider)
            VALUES (@OrderId, @CustomerId, @Street, @City, @Country, @PostCode, @PaymentProvider)";
        command.Parameters.AddWithValue("@OrderId", orderId.ToString().ToLowerInvariant());
        command.Parameters.AddWithValue("@CustomerId", order.CustomerId.ToString().ToLowerInvariant());
        command.Parameters.AddWithValue("@Street", order.Street);
        command.Parameters.AddWithValue("@City", order.City);
        command.Parameters.AddWithValue("@Country", order.Country);
        command.Parameters.AddWithValue("@PostCode", order.PostCode);
        command.Parameters.AddWithValue("@PaymentProvider", order.PaymentProvider);
        await command.ExecuteNonQueryAsync();

        var orderItems = new List<OrderItem>();
        Console.WriteLine($"[DEBUG] Inserting {order.OrderItems.Count} order items for order {orderId}");
        foreach (var item in order.OrderItems)
        {
            command.Parameters.Clear();
            var itemId = Guid.NewGuid();
            command.CommandText = @"
                INSERT INTO order_items (id, product_id, order_id, price_paid, name, short_description, quantity_ordered)
                VALUES (@ItemId, @ProductId, @OrderId, @PricePaid, @Name, @ShortDescription, @QuantityOrdered)";
            command.Parameters.AddWithValue("@ItemId", itemId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@ProductId", item.ProductId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@OrderId", orderId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@PricePaid", item.PricePaid);
            command.Parameters.AddWithValue("@Name", item.Name);
            command.Parameters.AddWithValue("@ShortDescription", item.ShortDescription);
            command.Parameters.AddWithValue("@QuantityOrdered", item.QuantityOrdered);
            await command.ExecuteNonQueryAsync();

            orderItems.Add(new OrderItem(
                itemId,
                item.ProductId,
                orderId,
                item.PricePaid,
                item.Name,
                item.ShortDescription,
                item.QuantityOrdered
            ));
        }

        return new OrderDTO(orderId, order.CustomerId, orderItems);
    }

    public async Task UpdateOrder(OrderDTO order)
    {
        using var command = await _dataAccess.CreateCommand();

        // Delete existing order items
        command.CommandText = "DELETE FROM order_items WHERE order_id = @OrderId";
        command.Parameters.AddWithValue("@OrderId", order.OrderId.ToString().ToLowerInvariant());
        await command.ExecuteNonQueryAsync();

        // Update order
        command.Parameters.Clear();
        command.CommandText = @"
            UPDATE orders 
            SET customer_id = @CustomerId
            WHERE id = @OrderId";
        command.Parameters.AddWithValue("@OrderId", order.OrderId.ToString().ToLowerInvariant());
        command.Parameters.AddWithValue("@CustomerId", order.CustomerId.ToString().ToLowerInvariant());
        await command.ExecuteNonQueryAsync();

        // Insert new order items
        foreach (var item in order.Items)
        {
            command.Parameters.Clear();
            command.CommandText = @"
                INSERT INTO order_items (id, product_id, order_id, price_paid, name, short_description, quantity_ordered)
                VALUES (@ItemId, @ProductId, @OrderId, @PricePaid, @Name, @ShortDescription, @QuantityOrdered)";
            command.Parameters.AddWithValue("@ItemId", item.Id.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@ProductId", item.ProductId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@OrderId", order.OrderId.ToString().ToLowerInvariant());
            command.Parameters.AddWithValue("@PricePaid", item.PricePaid);
            command.Parameters.AddWithValue("@Name", item.Name);
            command.Parameters.AddWithValue("@ShortDescription", item.ShortDescription);
            command.Parameters.AddWithValue("@QuantityOrdered", item.QuantityOrdered);
            await command.ExecuteNonQueryAsync();
        }
    }
} 
