using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.ViewModel;

public class PlaceOrderViewModel
{
    public IEnumerable<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
    [Required] public IEnumerable<PaymentProvider> PaymentProviders = Enum.GetValues<PaymentProvider>();
    [Required, DisplayName("Street")] public string OrderStreet { get; set; } = null!;
    [Required, DisplayName("City")] public string OrderCity { get; set; } = null!;
    [Required, DisplayName("Post Code")] public string OrderPostalCode { get; set; } = null!;
    [Required, DisplayName("Country")] public string OrderCountry { get; set; } = null!;

    [BindProperty]
    public PaymentProvider SelectedProvider { get; set; }

    [DisplayName("Subtotal")]
    public decimal Subtotal { get; set; }

    [DisplayName("Shipping Cost")]
    public decimal ShippingCost { get; set; }

    [DisplayName("Total")]
    public decimal Total { get; set; }
}

public enum PaymentProvider
{
    Visa,
    Mastercard,
    PayPal,
    GooglePay,
    ApplePay
}