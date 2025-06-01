using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

public class OrdersControllerController : Controller
{
    public IActionResult PlaceOrder()
    {
        return View();
    }
}