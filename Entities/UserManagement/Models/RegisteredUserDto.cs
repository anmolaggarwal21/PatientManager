using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.UserManagement.Models
{
	public  class RegisteredUserDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public GenderEnum Gender { get; set; }
		public string Password { get; set; }
		public string ReEnterPassword { get; set; }
		public string UserName { get; set; }

		public IdentityRole Role { get; set; }
	}
}
