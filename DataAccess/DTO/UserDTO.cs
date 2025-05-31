namespace DataAccess.DTO;

public record UserDTO(
    Guid Id,
    int Role,
    string FirstName,
    string LastName,
    string PasswordHash,
    string EmailAddress,
    string PhoneNumber
    );