using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebApplication1.Common;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

public class OrdersController(ProductsService products) : Controller
{
    public async Task<IActionResult> PlaceOrder()
    {
        var cookie = GetCookie();
        var foundProducts = await products.GetAllProducts(true);
        var idProductDict = foundProducts.Where(x => x.Id != null).ToDictionary(x => x.Id!.Value, x => x);
        var cartProducts = idProductDict.Where(x => cookie.CartIdCountDict.ContainsKey(x.Key)).Select(x => new CartItemViewModel
        {
            ProductId = x.Key,
            ProductName = x.Value.Name,
            Count = cookie.CartIdCountDict[x.Key],
            Price = x.Value.Price
        }).ToList();

        var viewModel = new PlaceOrderViewModel
        {
            Items = cartProducts
        };

        return View(viewModel);
    }

    // TODO - refactor this somewhere else
    // Duplicated in CartController
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