using System.Text.RegularExpressions;

namespace Domain;

public class EmailAddress
{
    private const string EMAIL_REGEX = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    
    public string Email { get; private set; }
    
    public EmailAddress(string email)
    {
        if (Regex.IsMatch(email, EMAIL_REGEX))
        {
            var errs = new Dictionary<string, string>();
            errs[nameof(Email)] = "Email address invaild";

            throw new DomainException(errs);
        }
        
        Email = email;
    }

    public override string ToString() => Email;
}