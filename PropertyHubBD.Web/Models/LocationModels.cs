using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyHubBD.Web.Models
{
    public class Division
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string SvgPath { get; set; } // For the map
        public string Color { get; set; } // For the map

        public ICollection<District> Districts { get; set; }
    }

    public class District
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int DivisionId { get; set; }
        public Division Division { get; set; }

        public ICollection<Upazilla> Upazillas { get; set; }
    }

    public class Upazilla
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
    }
}
