namespace PropertyHubBD.Web.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProperties { get; set; }
        public int PendingProperties { get; set; }
        public int TotalDivisions { get; set; }
        public List<Property> RecentProperties { get; set; } = new List<Property>();
        public List<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
    }
}
