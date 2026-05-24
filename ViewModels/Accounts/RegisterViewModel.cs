using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.ViewModels.Accounts
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required,EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16,ErrorMessage = "Password must be between 6 and 16 characters", MinimumLength = 6)] 
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
}
