using DeviceManager.Pages.User;
using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager
{
	public partial class UserDBContext : IdentityDbContext<Users>
    {
      
      
        public UserDBContext(DbContextOptions<UserDBContext> options)
            : base(options)
        {
        }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			string ADMIN_ID = "02174cf0–9412–4cfe-afbf-59f706d72cf6";
			string Admin_ROLE_ID =  "341743f0-asd2–42de-afbf-59kmkkmk72cd6";
			string Member_Role_id = "fbf6c1fa-d357-4a9a-ab52-eb0fc3ad7258";

			var adminRole = new IdentityRole
			{
				Name = "Admin",
				NormalizedName = "ADMIN",
				Id = Admin_ROLE_ID,
				ConcurrencyStamp = Admin_ROLE_ID
			};
			modelBuilder.Entity<IdentityRole>().HasData(adminRole);

			modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
			{
				Name = "Member",
				NormalizedName = "MEMBER",
				Id = Member_Role_id,
				ConcurrencyStamp = Member_Role_id
			});


			var appUser = new Entities.Users
            {  
                FirstName ="Super",
                LastName = "User",
                UserName ="SuperUser",
                Gender = "Male",
				NormalizedUserName = "SUPERUSER",
				Id = "8e445865-a24d-4543-a6c6-9443d048cdc9"
			};

			//set user password
			PasswordHasher<Users> ph = new PasswordHasher<Users>();
			appUser.PasswordHash = ph.HashPassword(appUser, "Test1234567!");

			//seed user
			modelBuilder.Entity<Users>().HasData(appUser);


			
			//set user role to admin
			modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
			{
				RoleId = Admin_ROLE_ID,
				UserId = "8e445865-a24d-4543-a6c6-9443d048cdc9", 
			});

			base.OnModelCreating(modelBuilder);
        }

		public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string userName, string[] roles)
		{
			UserManager<Users> _userManager = services.GetService<UserManager<Users>>();
			Users user = await  _userManager.FindByNameAsync(userName);
			var result = await _userManager.AddToRolesAsync(user, roles);

			return result;
		}
	}
}
