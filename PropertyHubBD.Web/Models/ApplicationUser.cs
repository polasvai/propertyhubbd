using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string UserType { get; set; } // "Admin", "Seller", "Reseller", "User"
        
        // Extended Profile Info
        public string? ProfilePictureUrl { get; set; }
        public string? Occupation { get; set; }
        public string? Bio { get; set; }
        public string? Gender { get; set; } // "Male", "Female", "Other"
        public int? Age { get; set; }
        
        // Helper properties
        public bool IsSuperAdmin => UserType == "Admin" && Email == "superadmin@propertyhubbd.com";
        public bool CanAddProperty => UserType == "Admin" || UserType == "Seller";
        public bool CanEditProperty => UserType == "Admin";
    }
}
