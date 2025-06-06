using System.Text.Json;
using DataAccess.DTO;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebApplication1.Common;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Route("products/orders")]
public class OrdersController : Controller
{
    private readonly ProductsService _productsService;
    private readonly IOrderRepository _orderRepository;
    private readonly ShippingService _shippingService;

    public OrdersController(
        ProductsService productsService,
        IOrderRepository orderRepository,
        ShippingService shippingService)
    {
        _productsService = productsService;
        _orderRepository = orderRepository;
        _shippingService = shippingService;
    }

    [HttpGet]
    public async Task<IActionResult> PlaceOrder()
    {
        var cookie = GetCookie();
        var foundProducts = await _productsService.GetAllProducts(true);
        var idProductDict = foundProducts.Where(x => x.Id != null).ToDictionary(x => x.Id!.Value, x => x);
        var cartProducts = idProductDict.Where(x => cookie.CartIdCountDict.ContainsKey(x.Key)).Select(x => new CartItemViewModel
        {
            ProductId = x.Key,
            ProductName = x.Value.Name,
            Count = cookie.CartIdCountDict[x.Key],
            Price = x.Value.Price
        }).ToList();

        var subtotal = cartProducts.Sum(x => x.Price * x.Count);
        var shippingCost = _shippingService.CalculateShippingCost(subtotal, cartProducts.Sum(x => x.Count));
        var total = subtotal + shippingCost;

        var viewModel = new PlaceOrderViewModel
        {
            Items = cartProducts,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            Total = total,
            PaymentProviders = Enum.GetValues<PaymentProvider>(),
            SelectedProvider = PaymentProvider.Visa // Default payment provider
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromForm] PlaceOrderViewModel order)
    {
        var cookie = GetCookie();
        var foundProducts = await _productsService.GetAllProducts(true);
        var idProductDict = foundProducts.Where(x => x.Id != null).ToDictionary(x => x.Id!.Value, x => x);
        var cartProducts = idProductDict.Where(x => cookie.CartIdCountDict.ContainsKey(x.Key)).Select(x => new CartItemViewModel
        {
            ProductId = x.Key,
            ProductName = x.Value.Name,
            Count = cookie.CartIdCountDict[x.Key],
            Price = x.Value.Price
        }).ToList();

        if (!ModelState.IsValid)
        {
            // Recalculate totals for the view
            order.Items = cartProducts;
            order.Subtotal = cartProducts.Sum(x => x.Price * x.Count);
            order.ShippingCost = _shippingService.CalculateShippingCost(order.Subtotal, cartProducts.Sum(x => x.Count));
            order.Total = order.Subtotal + order.ShippingCost;
            order.PaymentProviders = Enum.GetValues<PaymentProvider>();
            return View(order);
        }

        // Create order items from cart products
        var orderItems = cartProducts.Select(x => new NewOrderItemDto(
            x.ProductId,
            Guid.NewGuid(), // This will be replaced with the actual order ID
            x.ProductName,
            idProductDict[x.ProductId].ShortDescription,
            x.Count,
            (double)x.Price // Convert decimal to double
        )).ToList();

        // TODO: Get actual customer ID from authentication
        var customerId = Guid.Parse("ecf889e6-5ebd-48a0-860c-a15c56c0cf7f");

        // Create and save the order
        var newOrder = new NewOrderDTO(
            customerId,
            orderItems,
            order.OrderStreet,
            order.OrderCity,
            order.OrderCountry,
            order.OrderPostalCode,
            order.SelectedProvider.ToString()
        );
        await _orderRepository.AddOrder(newOrder);

        // Clear the cart
        Response.Cookies.Delete("shopping_cart");

        return RedirectToAction(nameof(Success));
    }

    [Route("success")]
    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }

    private CartCookie GetCookie()
    {
        if (!Request.Cookies.TryGetValue("shopping_cart", out var cookieString))
        {
            cookieString = JsonSerializer.Serialize(new CartCookie());
            Response.Cookies.Append("shopping_cart", cookieString);
        }

        return JsonSerializer.Deserialize<CartCookie>(cookieString)!;
    }
}