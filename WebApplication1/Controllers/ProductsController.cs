using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Route("/products")]
public class ProductsController(ProductsService products, ILogger<ProductsController> logger) : Controller
{
    public async Task<IActionResult> GetAllProducts()
    {
        var p = await products.GetAllProducts();
        return View(new ProductsViewModel(p));
    }

    [HttpGet]
    [Route("create")]
    public IActionResult CreateProduct()
    {
        return View();
    }

    [HttpPost]
    [Route("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductViewModel newProduct)
    {
        if (!ModelState.IsValid)
        {
            return View(newProduct);
        }

        await products.CreateProduct(
            newProduct.Name,
            newProduct.ShortDescription,
            newProduct.ShortDescription,
            newProduct.Price);

        return RedirectToAction(nameof(GetAllProducts));
    }
}