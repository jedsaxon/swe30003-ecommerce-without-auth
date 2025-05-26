namespace Domain;

public class Invoice
{
    public Guid? Id { get; private set; }
    
    public Order Order { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public InvoiceUser User { get; private set; }

    private Invoice(Guid? id, Order order, InvoiceStatus status, InvoiceUser user)
    {
        Id = id;
        Order = order;
        Status = status;
        User = user;
    }

    /// <summary>
    /// Creates a new, unpaid invoice using the order as a template. It will throw an ArgumentException in the event that
    /// order.Customer is null. If this is the case, it would be pointless to store the order in the database, so exception is
    /// thrown
    /// </summary>
    /// <param name="order">The order to create the invoice from</param>
    /// <exception cref="ArgumentNullException">If the the customer in the order is null.</exception>
    public static Invoice NewFromOrder(Order order)
    {
        if (order.Customer is null)
            throw new ArgumentNullException(nameof(order), "Cannot create invoice when the order does not have an assigned user.");
        
        var invoiceUser = InvoiceUser.CreateFromUser(order.Customer);
        return new Invoice(null, order, InvoiceStatus.StatusUnpaid, invoiceUser);
    }

    public static Invoice CreateExistingInvoice(Guid id, Order order, InvoiceStatus status, InvoiceUser user)
    {
        return new Invoice(id, order, status, user);
    }

    public void SetPaid()
    {
        Status = InvoiceStatus.StatusPaid;
    }

    public void SetCanceled()
    {
        Status = InvoiceStatus.StatusCanceled;
    }

    public void SetRefunded()
    {
        Status = InvoiceStatus.StatusRefunded;
    }
}