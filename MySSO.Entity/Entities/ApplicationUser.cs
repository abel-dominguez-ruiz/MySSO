using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MySSO.Entity.Entities
{
   public class ApplicationUser : IdentityUser
    {
        [MaxLength(255)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(255)]
        [Required]
        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
