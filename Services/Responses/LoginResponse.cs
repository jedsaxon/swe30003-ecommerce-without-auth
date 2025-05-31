using Domain;

namespace Services.Responses;

/// <summary>
/// When the user logs in, they can either be successfully logged in, or they failed. These are encapsulated
/// in the classes SuccessfulLoginResponse or FailedLoginAttempt (which both extend LoginResponse)
/// </summary>
public abstract class LoginResponse
{
    private LoginResponse() { }
    
    public class SuccessfulLoginResponse(User user) : LoginResponse
    {
        public User User { get; } = user;
    }

    public class FailedLoginAttempt : LoginResponse;
}

