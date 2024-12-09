using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignDesign_App.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactOrganizationType { get; set; }
        public int? MaintenanceSectionId { get; set; }
        [NotMapped]
        public string ContactFullName {
            get { return ContactFirstName + " " + ContactLastName; } 
        }
        //public string ContactRole { get; set; }
    }
}
