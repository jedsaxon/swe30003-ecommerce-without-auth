using DataAccess.DTO;

namespace DataAccess.Repositories;

public interface IUserRepository
{
    Task<List<UserDTO>> GetUsers();
    Task<UserDTO?> GetUser(Guid orderId);
    Task<UserDTO?> GetUser(string email);
    Task<UserDTO> AddUser(NewUserDTO e);
    Task UpdateUser(UserDTO e);
}