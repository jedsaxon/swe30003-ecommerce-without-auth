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
    private readonly OrderService _orderService;

    public OrdersController(
        ProductsService productsService,
        IOrderRepository orderRepository,
        ShippingService shippingService,
        OrderService orderService)
    {
        _productsService = productsService;
        _orderRepository = orderRepository;
        _shippingService = shippingService;
        _orderService = orderService;
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
        if (!ModelState.IsValid)
        {
            // Recalculate totals for the view
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
            order.Items = cartProducts;
            order.Subtotal = cartProducts.Sum(x => x.Price * x.Count);
            order.ShippingCost = _shippingService.CalculateShippingCost(order.Subtotal, cartProducts.Sum(x => x.Count));
            order.Total = order.Subtotal + order.ShippingCost;
            order.PaymentProviders = Enum.GetValues<PaymentProvider>();
            return View(order);
        }

        // Re-fetch cart from cookie/session for POST
        var cookiePost = GetCookie();
        var foundProductsPost = await _productsService.GetAllProducts(true);
        var idProductDictPost = foundProductsPost.Where(x => x.Id != null).ToDictionary(x => x.Id!.Value, x => x);
        var cartProductsPost = idProductDictPost.Where(x => cookiePost.CartIdCountDict.ContainsKey(x.Key)).Select(x => new CartItemViewModel
        {
            ProductId = x.Key,
            ProductName = x.Value.Name,
            Count = cookiePost.CartIdCountDict[x.Key],
            Price = x.Value.Price
        }).ToList();

        // Check stock before placing order
        foreach (var item in cartProductsPost)
        {
            var product = await _productsService.FindProductById(item.ProductId);
            if (product == null || product.Stock < item.Count)
            {
                ModelState.AddModelError("", $"Insufficient stock for product: {item.ProductName}");
                // Recalculate totals for the view
                order.Items = cartProductsPost;
                order.Subtotal = cartProductsPost.Sum(x => x.Price * x.Count);
                order.ShippingCost = _shippingService.CalculateShippingCost(order.Subtotal, cartProductsPost.Sum(x => x.Count));
                order.Total = order.Subtotal + order.ShippingCost;
                order.PaymentProviders = Enum.GetValues<PaymentProvider>();
                return View(order);
            }
        }

        // TODO: Get actual customer ID from authentication
        var customerId = Guid.Parse("ecf889e6-5ebd-48a0-860c-a15c56c0cf7f");
        var placeOrderDto = new PlaceOrderDTO
        {
            CustomerId = customerId,
            Items = cartProductsPost.Select(x => new NewOrderItemDto(
                x.ProductId,
                Guid.Empty, // Will be replaced in repo
                x.ProductName,
                string.Empty, // ShortDescription can be filled in service
                x.Count,
                (double)x.Price
            )).ToList(),
            Street = order.OrderStreet,
            City = order.OrderCity,
            Country = order.OrderCountry,
            PostCode = order.OrderPostalCode,
            PaymentProvider = order.SelectedProvider.ToString()
        };
        Console.WriteLine($"[DEBUG] Placing order with {placeOrderDto.Items.Count} items.");

        // Place the order
        await _orderService.PlaceOrder(placeOrderDto);

        // Decrease stock for each product
        foreach (var item in cartProductsPost)
        {
            var product = await _productsService.FindProductById(item.ProductId);
            if (product != null)
            {
                product.SetStock(product.Stock - item.Count);
                await _productsService.EditProduct(product.Id.Value, product.Name, product.ShortDescription, product.LongDescription, (double)product.Price, product.Listed, product.Stock);
            }
        }

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