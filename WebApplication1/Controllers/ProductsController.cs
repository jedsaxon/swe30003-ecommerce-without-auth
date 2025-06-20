using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using WebApplication1.Common;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Route("/products")]
public class ProductsController(ProductsService products) : Controller
{
    public async Task<IActionResult> GetAllProducts()
    {
        var userIsAdmin = false;

        var loggedInUser = Request.Cookies.GetLoggedInUser();
        if (loggedInUser is not null)
        {
            userIsAdmin = loggedInUser.Role == Role.AdministratorRole.Id;
        }
        
        Product[] p = await products.GetAllProducts(includeUnlisted: userIsAdmin);
        return View(new ProductsViewModel(p));
    }

    [HttpGet]
    [Route("create")]
    public IActionResult CreateProduct()
    {
        var loggedInUser = Request.Cookies.GetLoggedInUser();
        var userIsAdmin = loggedInUser is not null && loggedInUser.Role == Role.AdministratorRole.Id;
            
        if (!userIsAdmin)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }
        
        return View();
    }

    [HttpPost]
    [Route("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductViewModel newProduct)
    {
        var loggedInUser = Request.Cookies.GetLoggedInUser();
        var userIsAdmin = loggedInUser is not null && loggedInUser.Role == Role.AdministratorRole.Id;
            
        if (!userIsAdmin)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }
        
        if (!ModelState.IsValid)
        {
            return View(newProduct);
        }

        await products.CreateProduct(
            newProduct.Name,
            newProduct.ShortDescription,
            newProduct.ShortDescription,
            newProduct.Price,
            newProduct.Listed,
            newProduct.Stock);

        return RedirectToAction(nameof(GetAllProducts));
    }

    [HttpGet]
    [Route("edit")]
    public async Task<IActionResult> EditProduct([FromQuery] Guid productId)
    {
        var loggedInUser = Request.Cookies.GetLoggedInUser();
        var userIsAdmin = loggedInUser is not null && loggedInUser.Role == Role.AdministratorRole.Id;
            
        if (!userIsAdmin)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }
        
        var foundProduct = await products.FindProductById(productId);
        if (foundProduct is null) return NotFound();

        var editableProduct = new EditProductViewModel()
        {
            ProductId = productId,
            Name = foundProduct.Name,
            ShortDescription = foundProduct.ShortDescription,
            LongDescription = foundProduct.LongDescription,
            Price = foundProduct.Price,
            Listed = foundProduct.Listed,
            Stock = foundProduct.Stock
        };
        return View(editableProduct);
    }

    [HttpPost]
    [Route("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct([FromForm] EditProductViewModel editProduct)
    {
        var loggedInUser = Request.Cookies.GetLoggedInUser();
        var userIsAdmin = loggedInUser is not null && loggedInUser.Role == Role.AdministratorRole.Id;
            
        if (!userIsAdmin)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }
        
        if (!ModelState.IsValid)
        {
            return View(editProduct);
        }

        try
        {
            var result = await products.EditProduct(editProduct.ProductId, editProduct.Name, editProduct.ShortDescription,
                editProduct.LongDescription, (double)editProduct.Price, editProduct.Listed, editProduct.Stock);

            if (!result)
                ViewData["Error"] = "Unable to save results";
            else
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
        var loggedInUser = Request.Cookies.GetLoggedInUser();
        var userIsAdmin = loggedInUser is not null && loggedInUser.Role == Role.AdministratorRole.Id;
            
        if (!userIsAdmin)
        {
            return RedirectToAction(controllerName: "Account", actionName: "Login");
        }
        
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

    [HttpGet]
    [Route("{productId:guid}")]
    public async Task<IActionResult> ViewDetails(Guid productId)
    {
        if (!ModelState.IsValid)
        {
            return NotFound();
        }

        var foundProduct = await products.FindProductById(productId);
        if (foundProduct is null || foundProduct.Id is null)
        {
            return NotFound();
        }

        var vm = new ViewProductDetailsViewModel(
            foundProduct.Id ?? Guid.Empty,
            foundProduct.Name,
            foundProduct.LongDescription,
            foundProduct.ShortDescription,
            foundProduct.Price
        );
        return View(vm);
    }
}