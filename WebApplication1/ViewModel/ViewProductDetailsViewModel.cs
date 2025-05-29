namespace WebApplication1.ViewModel;

public record ViewProductDetailsViewModel(
    Guid ProductId,
    string ProductName,
    string ProductShortDescription,
    string ProductLongDescription,
    decimal Price);