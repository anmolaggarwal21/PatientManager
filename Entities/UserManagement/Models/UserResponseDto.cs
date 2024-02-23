using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.UserManagement.Models
{
	public class UserResponseDto
	{
        public bool Status { get; set; }
        public List<string> Errors { get; set; }

        public Users User { get; set; }

    }
}
