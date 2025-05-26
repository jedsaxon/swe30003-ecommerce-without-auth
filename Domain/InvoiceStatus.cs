namespace Domain;

public sealed class InvoiceStatus(string name)
{
    public static InvoiceStatus StatusPaid = new InvoiceStatus("Paid");
    public static InvoiceStatus StatusUnpaid = new InvoiceStatus("Unpaid");
    public static InvoiceStatus StatusRefunded = new InvoiceStatus("Refunded");
    public static InvoiceStatus StatusCanceled = new InvoiceStatus("Canceled");
    
    public string Name { get; private set; } = name;
}