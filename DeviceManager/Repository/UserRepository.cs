using DeviceManager.Pages.User;
using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Policy;

namespace DeviceManager.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly UserManager<Users> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UserRepository(UserManager<Users> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public async Task<UserResponseDto> AddUser(RegisteredUserDto userEntity)
		{
			UserResponseDto userResponseDto = new UserResponseDto();
			userResponseDto.Status = false;
			var existingUser = await _userManager.FindByNameAsync(userEntity.UserName);
			if (existingUser != null)
			{
				userResponseDto.Errors = new List<string>();
				userResponseDto.Errors.Add("Username already exists");
				return userResponseDto;
			}

			var userToBeCreated = new Users
			{
				FirstName = userEntity.FirstName,
				LastName = userEntity.LastName,
				UserName = userEntity.UserName,
				Gender = userEntity.Gender.ToString(),
				LockoutEnabled = false
				
			};
			var response = await _userManager.CreateAsync(userToBeCreated, userEntity.Password);

			if (response.Errors != null && response.Errors.Count() > 0)
            {
				
				userResponseDto.Status = false;
				userResponseDto.Errors = new List<string>();
				foreach (var item in response.Errors)
                {
					userResponseDto.Errors.Add(item.Description);

				}
				return userResponseDto;
            }
			else
			{
				await _userManager.AddToRoleAsync(userToBeCreated, userEntity.Role.Name);
            }
			userResponseDto.Status = true;
			return userResponseDto;
		}

		public async Task<bool> DeleteUser(string id)
		{
			var existingUser = await _userManager.FindByIdAsync(id);

			if (existingUser != null)
			{
				var userRole = await _userManager.GetRolesAsync(existingUser);
				await _userManager.RemoveFromRolesAsync(existingUser, userRole);
				var response = await _userManager.DeleteAsync(existingUser);

				if (response.Succeeded)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			return false;
		}

		public async Task<List<Users>> GetAllUser()
		{
			return await _userManager.Users.ToListAsync();
			
		}

		public async Task<string> GetRoleOfUser(Users user)
		{ 
			var role = (await _userManager.GetRolesAsync(user));
			if(role == null)
			{
				return "Member";
			}
			return role.First();
		}
		public async Task<Users?> GetUserById(string id)
		{
			var existingUser = await _userManager.FindByIdAsync(id);
			return existingUser != null ? existingUser : null;
		}

		public async Task<bool> UpdateUser(RegisteredUserDto userEntity)
		{
			
			var user = await GetUserByUsername(userEntity.UserName);
			var existingRole = await GetRoleOfUser(user);
			user.FirstName = userEntity.FirstName;
			user.LastName = userEntity.LastName;
			user.Gender = userEntity.Gender.ToString();
			
			var result = await _userManager.UpdateAsync(user);
			
			
			if (result.Succeeded) {
				if (existingRole != userEntity.Role.Name)
				{
					await _userManager.RemoveFromRoleAsync(user, existingRole);
					await _userManager.AddToRoleAsync(user, userEntity.Role.Name);
				}
				return true; 
			}
			else return false;

		}

		public async Task<Users> GetUserByUsername(string userName)
		{
			var existingUser = await _userManager.FindByNameAsync(userName);
			return existingUser;
		}

		public async Task<string> PasswordVerification(string password)
		{
			var passwordValidator = new PasswordValidator<Users>();
			var result = await passwordValidator.ValidateAsync(_userManager, null, password);

			if (result.Succeeded)
			{
				return null;
			}
			else
			{
				return result.Errors!.First().Description;
			}
		}

		public async Task<bool> AddRoles(string roleName)
		{
			var response = await _roleManager.CreateAsync(new IdentityRole
			{
				Name = roleName
			});
			if (response.Succeeded)
			{
				return true;
			}
			return false;

		}

		public async Task<List<IdentityRole>> GetRoles()
		{
			return await _roleManager.Roles.ToListAsync();
		}

		public async Task<UserResponseDto> AuthenticateUser(LoginUserDto loginUserDto)
		{
			UserResponseDto userResponseDto = new UserResponseDto();
			userResponseDto.Status = false;
			var user = await _userManager.FindByNameAsync(loginUserDto.Username);
			if (user == null) return userResponseDto;

			bool isValidUser = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);

			userResponseDto.Status = isValidUser;
			if (user != null) userResponseDto.User = user;
			return userResponseDto;

		}

		public async Task<bool> UpdateUser(Users user, string newPassword)
		{
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
			return result.Succeeded;
		}
	}
}
