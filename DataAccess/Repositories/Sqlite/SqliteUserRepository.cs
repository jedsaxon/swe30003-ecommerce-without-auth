using DataAccess.DTO;

namespace DataAccess.Repositories.Sqlite;

public class SqliteUserRepository(SqliteDataAccess dataAccess) : IUserRepository
{
    // Get all users
    public async Task<List<UserDTO>> GetUsers()
    {
        var users = new List<UserDTO>();

        var command = await dataAccess.CreateCommand();
        command.CommandText = "SELECT * FROM users";

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new UserDTO(
                reader.GetGuid(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6)
            ));
        }

        return users;
    }

    // Get a specific user by id
    public async Task<UserDTO?> GetUser(Guid userId)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE id = :id";
        command.Parameters.AddWithValue(":id", userId.ToString());

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserDTO(
                reader.GetGuid(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6)
            );
        }

        return null;
    }

    // Add a new user to the database
    public async Task<UserDTO> AddUser(NewUserDTO newUser)
    {
        var newId = Guid.NewGuid();

        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              INSERT INTO users (id, role_id, first_name, last_name, password_hash, email_address, phone_number)
                              VALUES (:id, :role, :first_name, :last_name, :password_hash, :email_address, :phone_number)
                              """;

        command.Parameters.AddWithValue(":id", newId.ToString());
        command.Parameters.AddWithValue(":role", newUser.Role);
        command.Parameters.AddWithValue(":first_name", newUser.FirstName);
        command.Parameters.AddWithValue(":last_name", newUser.LastName);
        command.Parameters.AddWithValue(":password_hash", newUser.HashedPassword);
        command.Parameters.AddWithValue(":email_address", newUser.EmailAddress);
        command.Parameters.AddWithValue(":phone_number", newUser.PhoneNumber);

        await command.ExecuteNonQueryAsync();

        return new UserDTO(newId, newUser.Role, newUser.FirstName, newUser.LastName, newUser.HashedPassword, newUser.EmailAddress, newUser.PhoneNumber);
    }

    // Update an existing user
    public async Task UpdateUser(UserDTO user)
    {
        var command = await dataAccess.CreateCommand();
        command.CommandText = """
                              UPDATE users
                              SET role_id = :role,
                                  first_name = :first_name,
                                  last_name = :last_name,
                                  password_hash = :password_hash,
                                  email_address = :email_address,
                                  phone_number = :phone_number
                              WHERE id = :id
                              """;

        command.Parameters.AddWithValue(":id", user.Id.ToString());
        command.Parameters.AddWithValue(":role", user.Role);
        command.Parameters.AddWithValue(":first_name", user.FirstName);
        command.Parameters.AddWithValue(":last_name", user.LastName);
        command.Parameters.AddWithValue(":password_hash", user.PasswordHash);
        command.Parameters.AddWithValue(":email_address", user.EmailAddress);
        command.Parameters.AddWithValue(":phone_number", user.PhoneNumber);

        await command.ExecuteNonQueryAsync();
    }}