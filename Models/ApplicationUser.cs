using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Models
{
    /// <summary>
    /// Identity user with display name and optional doctor profile fields.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        /// <summary>Populated for users in the Doctor role.</summary>
        public string? Qualification { get; set; }

        /// <summary>Populated for users in the Doctor role.</summary>
        public string? Specialization { get; set; }
    }
}
