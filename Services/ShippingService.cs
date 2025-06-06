namespace Services;

public class ShippingService
{
    private const decimal BaseShippingCost = 5.00m;
    private const decimal AdditionalItemCost = 1.00m;
    private const decimal FreeShippingThreshold = 50.00m;

    public decimal CalculateShippingCost(decimal subtotal, int totalItems)
    {
        if (subtotal >= FreeShippingThreshold)
        {
            return 0.00m;
        }

        return BaseShippingCost + (Math.Max(0, totalItems - 1) * AdditionalItemCost);
    }
} 