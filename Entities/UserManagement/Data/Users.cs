
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Entities
{
    public class Users : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        public string? LastName { get; set; } = null!;
     

        [MaxLength(6)]
        public string Gender { get; set; } = null!;
        public string? Address { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserAvatar { get; set; }

      

		/// <summary>
		/// Gets or sets the email address for this user.
		/// </summary>
		[ProtectedPersonalData]
		public override string? Email { get; set; }

		/// <summary>
		/// Gets or sets the normalized email address for this user.
		/// </summary>
		public override string? NormalizedEmail { get; set; }

	}
}
