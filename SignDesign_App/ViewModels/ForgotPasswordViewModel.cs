using System.ComponentModel.DataAnnotations;

namespace SignDesign_App.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
