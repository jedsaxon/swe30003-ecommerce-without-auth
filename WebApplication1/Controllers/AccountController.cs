using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Exceptions;
using Services.Responses;
using WebApplication1.Common;
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
    public async Task<IActionResult> Login([FromForm] LoginViewModel loginDetails)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDetails);
        }

        var response = await userService.AuthenticateUser(loginDetails.EmailAddress, loginDetails.Password);

        if (response is LoginResponse.FailedLoginAttempt)
        {
            ModelState.AddModelError("Other", "Email or password is invalid");
            return View(loginDetails);
        }
        
        if (response is LoginResponse.SuccessfulLoginResponse success)
        {
            var cookie = new AuthCookie
            {
                UserId = success.User.UserId.ToString() ??
                         throw new Exception("User ID could not be converted to string because it was not a GUID."),
                FirstName = success.User.FirstName,
                LastName = success.User.LastName,
                EmailAddress = success.User.EmailAddress.ToString(),
                Role = success.User.UserRole.Id,
                PhoneNumber = success.User.PhoneNumber.ToString()
            };

            Response.Cookies.Append("LoggedInUser", cookie.AsJson(), new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(1), // log user out in 1 day
            });

            return RedirectToAction(controllerName: "Home", actionName: "Index");
        }

        throw new ArgumentException("Login response was not successful nor failed.");
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

        // TODO - check for duplicate mail address

        try
        {
            await userService.CreateMemberAccount(
                createAccountDetails.FirstName,
                createAccountDetails.LastName,
                createAccountDetails.Password,
                createAccountDetails.EmailAddress,
                createAccountDetails.PhoneNumber
            );
        }
        catch (AccountExistsException)
        {
            ModelState.AddModelError("EmailAddress", "Account with this email already exists");
            return View(createAccountDetails);
        }
        catch (DomainException e)
        {
            ModelState.AddDomainExceptions(e);
            return View(createAccountDetails);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ViewAccount()
    {
        if (!Request.Cookies.ContainsKey("LoggedInUser"))
        {
            return RedirectToAction("Login");
        }
        
        return View();
    }

    [HttpPost]
    [Route("logout")]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("LoggedInUser");
        return RedirectToAction("Index", "Home");
    }
}