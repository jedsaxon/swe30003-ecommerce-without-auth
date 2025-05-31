namespace Domain;

/// <summary>
/// Contains details about the user who paid for the invoice. The user may
/// wish to delete their account. However, for legal reasons, it may be
/// reasonable to keep the user's details past that point. 
/// </summary>
public class InvoiceUser
{
    public User? MatchingUser { get; private set; }
    
    public string FirstName { get; private set; }

    public string LastName { get; private set; }
    
    public Address Address { get; private set; }
    
    public EmailAddress EmailAdderss { get; private set; }
    
    public PhoneNumber PhoneNumber { get; private set; }

    private InvoiceUser(PhoneNumber phoneNumber, EmailAddress emailAdderss, Address address, string lastName, string firstName, User? matchingUser)
    {
        PhoneNumber = phoneNumber;
        EmailAdderss = emailAdderss;
        Address = address;
        LastName = lastName;
        FirstName = firstName;
        MatchingUser = matchingUser;
    }

    public static InvoiceUser CreateFromUser(User user, Address orderAddress)
    {
        // We need no data validation, because it is assumed that the `User` class handles
        // all of it for us! No code duplication!
        
        return new InvoiceUser(
            user.PhoneNumber,
            user.EmailAddress,
            orderAddress,
            user.LastName,
            user.FirstName,
            user
        );
    }

    public static InvoiceUser CreateExisting(
        PhoneNumber phoneNumber, 
        EmailAddress emailAddress, 
        Address address, 
        string lastName, 
        string firstName, 
        User? matchingUser)
    {
        return new InvoiceUser(
            phoneNumber, 
            emailAddress, 
            address, 
            lastName, 
            firstName, 
            matchingUser);
    }
}