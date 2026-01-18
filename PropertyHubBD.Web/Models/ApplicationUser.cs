using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string UserType { get; set; } // "Admin", "Seller", "Reseller", "User"
        
        // Helper properties
        public bool IsSuperAdmin => UserType == "Admin" && Email == "superadmin@propertyhubbd.com";
        public bool CanAddProperty => UserType == "Admin" || UserType == "Seller";
        public bool CanEditProperty => UserType == "Admin";
    }
}
