using System.Runtime.InteropServices.Marshalling;
using System.Security.Principal;
using DataAccess.DTO;
using DataAccess.Repositories;
using Domain;

namespace Services;

public class UserService(IUserRepository userRepository)
{
    public async Task CreateMemberAccount(string firstName, string lastName, string password, string emailAddress, string phoneNumber)
    {
        var user = User.CreateNewUser(Role.MemberRole, firstName, lastName, password, emailAddress, phoneNumber);
        await CreateAccount(user);
    }
    
    public async Task CreateAdminAccount(string firstName, string lastName, string password, string emailAddress, string phoneNumber) 
    {
        var user = User.CreateNewUser(Role.AdministratorRole, firstName, lastName, password, emailAddress, phoneNumber);
        await CreateAccount(user);
    }

    private async Task CreateAccount(User user)
    {
        var newUserDto = new NewUserDTO(
            user.UserRole.Id, 
            user.FirstName, 
            user.LastName, 
            user.EmailAddress.ToString(), 
            user.PhoneNumber.ToString());
        
        await userRepository.AddUser(newUserDto);
    }
}