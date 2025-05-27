namespace Domain;

public class Product
{
    private const int NAME_MAX_LENGTH = 256;
    private const int SHORT_DESC_MAX_LENGTH = 256;
    private const int LONG_DESC_MAX_LENGTH = 4096;

    /// <summary>
    /// The ID of the object in the database. If this is null, it means it has not been saved into the database yet
    /// </summary>
    public Guid? Id { get; private set; }
    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescription { get; private set; }
    public decimal Price { get; private set; }

    /// <exception cref="DomainException">If there were any business rules violated</exception>
    private Product(Guid? id, string name, string shortDescription, string longDescription, decimal price)
    {
        var errors = new Dictionary<string, string>();

        if (name.Length == 0) errors[nameof(Name)] = "Name cannot be empty";
        if (name.Length > NAME_MAX_LENGTH) errors[nameof(Name)] = $"Name cannot be greater than {NAME_MAX_LENGTH} characters";
        if (shortDescription.Length > SHORT_DESC_MAX_LENGTH)
            errors[nameof(ShortDescription)] = $"Description cannot be greater than {SHORT_DESC_MAX_LENGTH} characters";
        if (longDescription.Length > LONG_DESC_MAX_LENGTH)
            errors[nameof(LongDescription)] = $"Description canont be greater than {LONG_DESC_MAX_LENGTH} characters";
        if (price <= 0) errors[nameof(Price)] = $"Price cannot be lower than $0.00";

        if (errors.Count > 0) throw new DomainException(errors);

        Id = id;
        Name = name;
        ShortDescription = shortDescription;
        LongDescription = longDescription;
        Price = price;
    }

    /// <summary>
    /// Creates a new product that does not yet exist in the database (`Id` will be set to `null` by default).
    /// Will perform validation to ensure the details are correct.
    /// </summary>
    /// <returns>The newly created product with no identity</returns>
    /// <exception cref="DomainException">If there were any business rules violated</exception>
    public static Product NewProduct(string name, string shortDescription, string longDescription, decimal price)
    {
        return new Product(null, name, shortDescription, longDescription, price);
    }

    /// <summary>
    /// Returns an existing product that does exist in the database, who's ID is set by the database.
    /// </summary>
    /// <returns>An existing product with an identity</returns>
    /// <exception cref="DomainException">If there were any business rules violated</exception>
    public static Product ProductWithIdentity(
        Guid id,
        string name,
        string shortDescription,
        string longDescription,
        decimal price)
    {
        return new Product(id, name, shortDescription, longDescription, price);
    }

    /// <summary>
    /// Renames the product
    /// </summary>
    /// <exception cref="ArgumentException">If the length is less than 0 or greater than 256</exception>
    public void Rename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(newName.Length, NAME_MAX_LENGTH);
        Name = newName;
    }

    /// <summary>
    /// Sets the short description
    /// </summary>
    /// <exception cref="ArgumentException">Throws if short description length is 0 or greater than 256</exception>
    public void SetShortDescription(string shortDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(shortDescription);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(shortDescription.Length, SHORT_DESC_MAX_LENGTH);
        ShortDescription = shortDescription;
    }

    /// <summary>
    /// Sets the long description
    /// </summary>
    /// <exception cref="ArgumentException">Throws if long description length is 0 or greater than 4096</exception>
    public void SetLongDescription(string longDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(longDescription);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(longDescription.Length, LONG_DESC_MAX_LENGTH);
        LongDescription = longDescription;
    }

    /// <summary>
    /// Updates the price of the product
    /// </summary>
    /// <exception cref="ArgumentException">Throws if price is 0 </exception>
    public void UpdatePrice(double price)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(price, 0);
    }
}