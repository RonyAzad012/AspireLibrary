using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AspireLibrary.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Profile Image URL")]
        public string ProfileImageUrl { get; set; }
    }
}
