namespace WebApplication1.Common;

/// <summary>
/// When a user logs in, they need to store their details in a cookie so the site can display it correctly. This cookie is
/// therefore created to store user details and be serializable by json
/// </summary>
public class AuthCookie
{
    /// <summary>
    /// A GUID in string form
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string EmailAddress { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;
}