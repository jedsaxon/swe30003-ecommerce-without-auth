namespace DataAccess.DTO;

public record NewUserDTO(
    Guid Id,
    string Role,
    string FirstName,
    string LastName,
    string EmailAddress,
    string PhoneNumber,
    string AddressStreet,
    string AddressCity,
    string AddressPostalCode,
    string AddressCountry
);