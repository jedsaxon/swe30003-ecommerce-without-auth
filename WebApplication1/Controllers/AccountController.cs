using Microsoft.AspNetCore.Mvc;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Microsoft.AspNetCore.Components.Route("/account")]
public class AccountController : Controller
{
    [HttpGet]
    [Route("login")]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromForm] LoginViewModel loginDetails)
    {
        return RedirectToAction(controllerName: "Home", actionName: "Index");
    }

    [HttpGet]
    [Route("create")]
    public IActionResult CreateAccount()
    {
        return View(new CreateAccountViewModel());
    }

    [HttpPost]
    [Route("create")]
    public IActionResult CreateAccount([FromForm] CreateAccountViewModel createAccountDetails)
    {
        return RedirectToAction(nameof(Login));
    }
}