using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Models
{
    public class HomeViewModel
    {
        public List<DivisionViewModel> Divisions { get; set; } = new();
        public List<Property> Properties { get; set; } = new();
        public int? SelectedDivisionId { get; set; }
        public string SearchTerm { get; set; }
    }
}
