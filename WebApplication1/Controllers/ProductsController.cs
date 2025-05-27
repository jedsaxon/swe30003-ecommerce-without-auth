using Microsoft.AspNetCore.Mvc;
using Services;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[ApiController]
[Route("/products")]
public class ProductsController(ProductsService products) : Controller
{
    public async Task<IActionResult> GetAllProducts()
    {
        var p = await products.GetAllProducts();
        return View(new ProductsViewModel(p));
    }
}