using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace DeviceManager.Repository
{
	public interface IUserRepository
	{
		Task<UserResponseDto> AddUser(RegisteredUserDto userEntity);
		Task<bool> DeleteUser(string id);
		Task<List<Users>> GetAllUser();
		Task<Users?> GetUserById(string id);
		Task<bool> UpdateUser(RegisteredUserDto userEntity);
		Task<Users> GetUserByUsername(string userName);
		Task<bool> AddRoles(string roleName);
		Task<string> PasswordVerification(string password);

		Task<List<IdentityRole>> GetRoles();
		Task<string> GetRoleOfUser(Users user);
		Task<UserResponseDto> AuthenticateUser(LoginUserDto loginUserDto);
		Task<bool> UpdateUserPassword(Users user, string newPassword, bool isPasswordResetByAdmin = true);

    }
}
