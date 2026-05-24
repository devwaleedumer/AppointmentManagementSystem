using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        public string? Qualification { get; set; }
        public string? Specialization { get; set; }
    }
}
