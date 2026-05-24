using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.ViewModels.Accounts
{
    public class LoginViewModel
    {
        [EmailAddress,Required(ErrorMessage = "Email address is required"),Display(Name = "Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required"), DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
