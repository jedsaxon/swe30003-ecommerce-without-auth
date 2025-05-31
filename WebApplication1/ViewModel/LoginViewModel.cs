using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel;

public class LoginViewModel
{
    [Required, EmailAddress] public string EmailAddress { get; set; } = null!;

    [Required, PasswordPropertyText] public string Password { get; set; } = null!;
}