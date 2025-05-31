using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Route("/products")]
public class ProductsController(ProductsService products) : Controller
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
    public async Task<IActionResult> EditProduct([FromQuery] Guid productId)
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

    [HttpPost]
    [Route("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct([FromForm] EditProductViewModel editProduct)
    {
        if (!ModelState.IsValid)
        {
            return View(editProduct);
        }

        try
        {
            await products.EditProduct(editProduct.ProductId, editProduct.Name, editProduct.ShortDescription,
                editProduct.LongDescription, (double)editProduct.Price);
            ViewData["Success"] = "Results saved successfully.";
        }
        catch
        {
            ViewData["Error"] = "Could not save new product details.";
        }

        return View(editProduct);
    }

    [HttpGet]
    [Route("delete")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Error"] = "Unable to delete product - not found";
            return RedirectToAction(nameof(EditProduct), productId);
        }

        if (!await products.DeleteProduct(productId))
        {
            ViewData["Error"] = "Unable to delete product - not found";
            return RedirectToAction(nameof(EditProduct), productId);
        }

        ViewData["Success"] = "Product deleted successfully";
        return RedirectToAction(nameof(GetAllProducts));
    }
}