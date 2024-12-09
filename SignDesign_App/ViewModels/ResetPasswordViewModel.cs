using System.ComponentModel.DataAnnotations;

namespace SignDesign_App.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
