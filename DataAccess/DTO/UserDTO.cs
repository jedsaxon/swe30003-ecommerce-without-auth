namespace DataAccess.DTO;

public record UserDTO(
    Guid Id,
    int Role,
    string FirstName,
    string LastName,
    string EmailAddress,
    string PhoneNumber
    );