using Microsoft.AspNetCore.Mvc;
using Services;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

[Microsoft.AspNetCore.Components.Route("/account")]
public class AccountController(UserService userService) : Controller
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
        if (!ModelState.IsValid)
        {
            return View(loginDetails);
        }
        
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
    public async Task<IActionResult> CreateAccount([FromForm] CreateAccountViewModel createAccountDetails)
    {
        if (!ModelState.IsValid)
        {
            return View(createAccountDetails);
        }

        await userService.CreateMemberAccount(
            createAccountDetails.FirstName,
            createAccountDetails.LastName,
            createAccountDetails.Password,
            createAccountDetails.EmailAddress,
            createAccountDetails.PhoneNumber
        );

        return RedirectToAction(nameof(Login));
    }
}