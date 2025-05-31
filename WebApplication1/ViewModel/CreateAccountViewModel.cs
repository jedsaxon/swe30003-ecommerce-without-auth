using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel;

public class CreateAccountViewModel
{
    [Required, MaxLength(64)] public string FirstName { get; set; } = null!;

    [Required, MaxLength(64)] public string LastName { get; set; } = null!;
    
    [Required, EmailAddress] public string EmailAddress { get; set; } = null!;

    [Required, Phone] public string PhoneNumber { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
}