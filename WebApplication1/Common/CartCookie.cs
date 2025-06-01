namespace WebApplication1.Common;

public record CartCookie
{
    public IReadOnlyDictionary<Guid, int> CartIdCountDict { get; init; } = new Dictionary<Guid, int>();
}