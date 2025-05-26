using System.Text.RegularExpressions;

namespace Domain;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string city, string postalCode, string country)
    {
        var exceptions = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(street))
            exceptions.Add(nameof(Street), "Street cannot be empty.");
        if (string.IsNullOrEmpty(city))
            exceptions.Add(nameof(City), "City cannot be empty.");
        if (string.IsNullOrEmpty(postalCode))
            exceptions.Add(nameof(PostalCode), "Postal code cannot be empty.");


        var regex = new Regex(@"^[A-Za-z0-9\s\-]+$");
        if (!regex.IsMatch(postalCode))
            exceptions.Add(nameof(PostalCode), "Postal code format is invalid.");

        if (string.IsNullOrEmpty(country))
            exceptions.Add(nameof(Country), "Country cannot be empty.");

        if (exceptions.Count > 0)
            throw new DomainException(exceptions);

        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }
}