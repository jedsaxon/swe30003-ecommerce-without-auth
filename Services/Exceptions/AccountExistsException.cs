namespace Services.Exceptions;

/// <summary>
/// Thrown if an account with similar details already exists. For example, user creates 2 accounts with the same email address.
/// </summary>
public class AccountExistsException : Exception
{
       
}