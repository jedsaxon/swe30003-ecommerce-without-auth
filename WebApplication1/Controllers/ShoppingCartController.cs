using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebApplication1.Common;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Route("/cart")]
public class ShoppingCartController(ProductsService productsService) : Controller
{
    private const string CookieName = "shopping_cart";

    [HttpGet]
    [Route("{productId:guid}/add")]
    public async Task<IActionResult> Add([FromRoute] Guid productId)
    {       
        var product = await productsService.FindProductById(productId);
        if (product == null)
        {
            return NotFound();
        }

        var cookie = GetCookie();
        var newDict = cookie.CartIdCountDict.ToDictionary();
        if (!newDict.TryAdd(productId, 1))
        {
            newDict[productId] = newDict[productId] > 0 ? newDict[productId] + 1 : 1;
        }
        
        Response.Cookies.Append(CookieName, JsonSerializer.Serialize(new CartCookie
        {
            CartIdCountDict = newDict
        }));

        return RedirectToAction("ViewShoppingCart");
    }
    
    [HttpGet]
    [Route("{productId:guid}/delete")]
    public IActionResult Delete([FromRoute] Guid productId)
    {       
        var cookie = GetCookie();
        var newDict = cookie.CartIdCountDict.ToDictionary();
        if (!newDict.ContainsKey(productId))
        {
            return RedirectToAction("ViewShoppingCart");
        }

        newDict[productId]--;
        if (newDict[productId] < 1)
        {
            newDict.Remove(productId);
        }
        
        Response.Cookies.Append(CookieName, JsonSerializer.Serialize(new CartCookie
        {
            CartIdCountDict = newDict
        }));

        return RedirectToAction("ViewShoppingCart");
    }
    
    [HttpGet]
    [Route("{productId:guid}/remove")]
    public IActionResult Remove([FromRoute] Guid productId)
    {       
        var cookie = GetCookie();
        var newDict = cookie.CartIdCountDict.ToDictionary();
        if (!newDict.ContainsKey(productId))
        {
            return RedirectToAction("ViewShoppingCart");
        }
        
        newDict.Remove(productId);
        
        Response.Cookies.Append(CookieName, JsonSerializer.Serialize(new CartCookie
        {
            CartIdCountDict = newDict
        }));

        return RedirectToAction("ViewShoppingCart");
    }
    
    [HttpGet]
    [Route("clear")]
    public IActionResult Clear()
    {
        Response.Cookies.Append(CookieName, JsonSerializer.Serialize(new CartCookie()));

        return RedirectToAction("ViewShoppingCart");
    }

    [HttpGet]
    public async Task<IActionResult> ViewShoppingCart()
    {
        var cookie = GetCookie();
        var products = await productsService.GetAllProducts(true);
        var idProductDict = products.Where(x => x.Id != null).ToDictionary(x => x.Id!.Value, x => x);
        var cartProducts = idProductDict.Where(x => cookie.CartIdCountDict.ContainsKey(x.Key)).Select(x => new CartItemViewModel
        {
            ProductId = x.Key,
            ProductName = x.Value.Name,
            Count = cookie.CartIdCountDict[x.Key]
        }).ToHashSet();
        var viewModel = new ViewShoppingCartViewModel
        {
            Items = cartProducts
        };

        return View(viewModel);
    }

    private CartCookie GetCookie()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var cookieString))
        {
            cookieString = JsonSerializer.Serialize(new CartCookie());
            Response.Cookies.Append(CookieName, cookieString);
        }

        return JsonSerializer.Deserialize<CartCookie>(cookieString)!;
    }
}