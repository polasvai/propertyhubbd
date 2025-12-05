using Microsoft.AspNetCore.Identity;

namespace PropertyHubBD.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string UserType { get; set; } // "Admin", "Seller", "Reseller", "User"
    }
}
