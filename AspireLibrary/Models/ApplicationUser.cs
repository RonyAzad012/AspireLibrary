using Microsoft.AspNetCore.Identity;

namespace AspireLibrary.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FullName { get; set; }
        public required string Role { get; set; }
        public string? ProfileImage { get; set; }
    }

}
