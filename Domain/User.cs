namespace Domain;

public class User
{
    public Guid? UserId { get; private set; }
    
    public Role UserRole { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }
    
    public EmailAddress EmailAddress { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public Address Address { get; private set; }

    private User(Guid? userId,
        Role userRole,
        string firstName,
        string lastName,
        EmailAddress emailAddress,
        PhoneNumber phoneNumber, 
        Address address)
    {
        UserId = userId;
        UserRole = userRole;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public static User CreateNewUser(
        Role userRole, 
        string firstName, 
        string lastName, 
        string emailAddress, 
        string phoneNumber,
        string addressStreet,
        string addressCity,
        string addressPostalCode,
        string addressCountry)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(firstName)) errors[nameof(FirstName)] = "First name cannot be empty";
        else if (firstName.Length > 64) errors[nameof(FirstName)] = "First name cannot be greater than 64 characters";

        if (string.IsNullOrWhiteSpace(lastName)) errors[nameof(LastName)] = "Last name cannot be empty";
        else if (lastName.Length > 64) errors[nameof(LastName)] = "Last name cannot be greater than 64 characters";

        PhoneNumber? number = null;
        DomainException.TryExecute(errors, () => number = new PhoneNumber(phoneNumber));

        EmailAddress? emailAddr = null;
        DomainException.TryExecute(errors, () => emailAddr = new EmailAddress(emailAddress));

        Address? addr = null;
        DomainException.TryExecute(errors, () => addr = new Address(addressStreet, addressCity, addressPostalCode, addressCountry));

        if (errors.Count > 0)
            throw new DomainException(errors);

        return new User(null, userRole, firstName, lastName, emailAddr!, number!, addr!);
    }

    public static User CreateExisting(
        Guid userId,
        Role userRole,
        string firstName,
        string lastName,
        EmailAddress emailAddress,
        PhoneNumber phoneNumber,
        Address address
    )
    {
        return new User(null, userRole, firstName, lastName, emailAddress, phoneNumber, address);
    }
}