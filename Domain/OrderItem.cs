namespace Domain;

public class OrderItem
{
    /// <summary>
    /// The ID of the object in the database. If this is null, it means it has not been saved into the database yet
    /// </summary>
    public Guid? Id { get; private set; }

    /// <summary>
    /// Used as a link between the order item and product. Is optional incase the product owner wants to completely
    /// de-list the product.
    /// </summary>
    public Product? Product { get; private set; }

    public decimal PricePaid { get; private set; }
    
    public string Name { get; private set; }
    
    public string ShortDescription { get; private set; }

    public int Quantity { get; private set; }

    /// <summary>
    /// Creates a new OrderItem instance using the product as a template
    /// </summary>
    public static OrderItem FromProduct(Product p, int quantity)
    {
        return new OrderItem(null, p, p.Price, p.Name, p.ShortDescription, quantity);
    }

    public static OrderItem OrderItemWithIdentity(
        Guid id,
        Product? product,
        decimal pricePaid,
        string name,
        string shortDescription,
        int quantity)
    {
        return new OrderItem(id, product, pricePaid, name, shortDescription, quantity);
    }

    private OrderItem(Guid? id, Product? product, decimal pricePaid, string name, string shortDescription, int quantity)
    {
        // TODO - create custom class for product name, description, short description, and price

        // Makes the code more complex, but OOP magic or something

        Id = id;
        Product = product;
        PricePaid = pricePaid;
        Name = name;
        ShortDescription = shortDescription;
        Quantity = quantity;
    }
}