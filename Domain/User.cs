namespace Domain;

public class User
{
    public Guid? UserId { get; private set; }
    
    public Role UserRole { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Password { get; private set; }
    
    public EmailAddress EmailAddress { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    private User(Guid? userId,
        Role userRole,
        string firstName,
        string lastName,
        string password,
        EmailAddress emailAddress,
        PhoneNumber phoneNumber)
    {
        UserId = userId;
        UserRole = userRole;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
    }

    public static User CreateNewUser(
        Role userRole, 
        string firstName, 
        string lastName, 
        string password,
        string emailAddress, 
        string phoneNumber)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(firstName)) errors[nameof(FirstName)] = "First name cannot be empty";
        else if (firstName.Length > 64) errors[nameof(FirstName)] = "First name cannot be greater than 64 characters";

        if (string.IsNullOrWhiteSpace(lastName)) errors[nameof(LastName)] = "Last name cannot be empty";
        else if (lastName.Length > 64) errors[nameof(LastName)] = "Last name cannot be greater than 64 characters";

        if (string.IsNullOrWhiteSpace(password)) errors[nameof(Password)] = "Password cannot be empty";

        PhoneNumber? number = null;
        DomainException.TryExecute(errors, () => number = new PhoneNumber(phoneNumber));

        EmailAddress? emailAddr = null;
        DomainException.TryExecute(errors, () => emailAddr = new EmailAddress(emailAddress));

        if (errors.Count > 0)
            throw new DomainException(errors);

        return new User(null, userRole, firstName, lastName, password, emailAddr!, number!);
    }

    public static User CreateExisting(
        Guid userId,
        Role userRole,
        string firstName,
        string lastName,
        string password,
        EmailAddress emailAddress,
        PhoneNumber phoneNumber
    )
    {
        return new User(userId, userRole, firstName, lastName, password, emailAddress, phoneNumber);
    }
}