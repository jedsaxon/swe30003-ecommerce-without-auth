using System.Runtime.InteropServices.Marshalling;
using System.Security.Principal;
using DataAccess.DTO;
using DataAccess.Repositories;
using Domain;
using Services.Exceptions;
using Services.Responses;

namespace Services;

public class UserService(IUserRepository userRepository)
{
    /// <exception cref="AccountExistsException">If the account already exists</exception>
    public async Task CreateMemberAccount(string firstName, string lastName, string password, string emailAddress, string phoneNumber)
    {
        var user = User.CreateNewUser(Role.MemberRole, firstName, lastName, password, emailAddress, phoneNumber);
        await CreateAccount(user);
    }
    
    /// <exception cref="AccountExistsException">If the account already exists</exception>
    public async Task CreateAdminAccount(string firstName, string lastName, string password, string emailAddress, string phoneNumber) 
    {
        var user = User.CreateNewUser(Role.AdministratorRole, firstName, lastName, password, emailAddress, phoneNumber);
        await CreateAccount(user);
    }

    /// <exception cref="AccountExistsException">If the account already exists</exception>
    private async Task CreateAccount(User user)
    {
        var existingUser = await FindUserByEmail(user.EmailAddress.ToString());

        if (existingUser is not null)
        {
            throw new AccountExistsException();
        }
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
        
        var newUserDto = new NewUserDTO(
            user.UserRole.Id, 
            user.FirstName, 
            user.LastName, 
            hashedPassword,
            user.EmailAddress.ToString(), 
            user.PhoneNumber.ToString());
        
        await userRepository.AddUser(newUserDto);
    }

    public async Task<LoginResponse> AuthenticateUser(string emailAddress, string password)
    {
        var foundUser = await FindUserByEmail(emailAddress);

        if (foundUser is null || !BCrypt.Net.BCrypt.Verify(password, foundUser.Password))
        {
            return new LoginResponse.FailedLoginAttempt();
        }

        return new LoginResponse.SuccessfulLoginResponse(foundUser);
    }

    public async Task<User?> FindUserByEmail(string emailAddress)
    {
        var user = await userRepository.GetUser(emailAddress);
        if (user is null) return null;

        var role = Role.FromId(user.Role);

        if (role is null)
            throw new ArgumentNullException($"The User {user.Id} has an invalid role ({user.Role}) which does not exist.");

        return User.CreateExisting(
            user.Id,
            role,
            user.FirstName,
            user.LastName,
            user.PasswordHash,
            new EmailAddress(user.EmailAddress),
            new PhoneNumber(user.PhoneNumber)
            );
    }
}