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

    [HttpGet]
    [Route("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(Guid productId)
    {
        var foundProduct = await products.FindProductById(productId);
        if (foundProduct is null) return NotFound();

        var editableProduct = new EditProductViewModel()
        {
            ProductId = productId,
            Name = foundProduct.Name,
            ShortDescription = foundProduct.ShortDescription,
            LongDescription = foundProduct.LongDescription,
            Price = foundProduct.Price
        };
        return View(editableProduct);
    }

    [HttpPut]
    [Route("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct([FromBody] EditProductViewModel editProduct)
    {
        if (!ModelState.IsValid)
        {
            return View(editProduct);
        }

        bool result = await products.EditProduct(editProduct.ProductId, editProduct.Name, editProduct.ShortDescription,
            editProduct.LongDescription, (double)editProduct.Price);

        if (!result)
        {
            ViewData["Error"] = "Could not save new product details.";
            return View(editProduct);
        }

        return View(editProduct.ProductId);
    }
}