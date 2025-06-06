namespace Services;

public class ShippingService
{
    private const decimal RatePerItem = 2.00m;

    public decimal CalculateShippingCost(decimal subtotal, int totalItems)
    {
        return RatePerItem * totalItems;
    }
} 