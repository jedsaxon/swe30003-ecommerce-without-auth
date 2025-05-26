namespace Domain;

public class Order
{
    public Guid? OrderId { get; private set; }
    
    public User? Customer { get; private set; }

    public List<OrderItem> OrderItems { get; private set; }

    /// <summary>
    /// The address the order will be shipped to. This does not have to match the address of the user. 
    /// </summary>
    public Address ShippingAddress { get; private set; }

    private Order(Guid? orderId, User? customer, List<OrderItem> items, Address address)
    {
        OrderId = orderId;
        Customer = customer;
        OrderItems = items;
        ShippingAddress = address;
    }

    /// <summary>
    /// Creates a new order based on the user's details and the shopping cart. In case the user wants to use a different
    /// address to ship, `customer.Address` is not used, in favour of a separate argument.
    /// </summary>
    public static Order CreateNewOrder(User customer, ShoppingCart cart, Address shipTo)
    {
        List<OrderItem> orderItems = cart
            .Select(cartItem => OrderItem.FromProduct(cartItem.Product, cartItem.Quantity))
            .ToList();

        return new Order(null, customer, orderItems, shipTo);
    }

    /// <summary>
    /// Creates a new order that already exists in the database and has an identifier
    /// </summary>
    public static Order CreateWithIdentity(Guid orderId, User? customer, List<OrderItem> items, Address address)
    {
        return new Order(orderId, customer, items, address);
    }
}