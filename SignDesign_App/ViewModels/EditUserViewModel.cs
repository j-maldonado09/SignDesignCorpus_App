using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SignDesign_App.ViewModels
{
    public class EditUserRoleViewModel
    {
        public string id { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        [Display(Name = "First Name")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [Display(Name = "Last Name")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter organization type")]
        [Display(Name = "Organization")]
        [MaxLength(30)]
        public string OrganizationType { get; set; }

        [Required(ErrorMessage = "Please enter your maintenance section")]
        [DataType(DataType.Text)]
        [Display(Name = "Maintenance Section")]
        public int? MaintenanceSectionId { get; set; }

        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Role")]
        public string Group { get; set; }

        public string PreviousGroup { get; set; }

        [Display(Name = "Role")]
        public IQueryable<IdentityRole> Groups { get; set; }
    }
}
