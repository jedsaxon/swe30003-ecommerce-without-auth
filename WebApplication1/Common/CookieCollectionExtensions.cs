using System.Text.Json;

namespace WebApplication1.Common;

public static class CookieCollectionExtensions
{
    public static AuthCookie? GetLoggedInUser(this IRequestCookieCollection cookies)
    {
        return cookies.TryGetValue("LoggedInUser", out var json) ? AuthCookie.FromJson(json) : null;
    }
}