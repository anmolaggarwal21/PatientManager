using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.UserManagement.Models
{
    public class DisplayUser
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; } 


        [MaxLength(6)]
        public string? Gender { get; set; } 

        public string? UserName { get; set; }

        public string? RoleName { get; set; }

        public string? Id { get; set; }

    }
}
