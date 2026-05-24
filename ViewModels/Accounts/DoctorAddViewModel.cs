using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.ViewModels.Accounts
{
    public class DoctorAddViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Qualification { get; set; }
        [Required]
        public string? Specialization { get; set; }
    }

}
