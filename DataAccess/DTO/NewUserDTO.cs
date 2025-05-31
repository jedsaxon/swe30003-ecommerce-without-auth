namespace DataAccess.DTO;

public record NewUserDTO(
    int Role,
    string FirstName,
    string LastName,
    string EmailAddress,
    string PhoneNumber
);