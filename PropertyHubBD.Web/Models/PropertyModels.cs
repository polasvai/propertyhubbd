using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyHubBD.Web.Models
{
    public class Property
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public double AreaSize { get; set; } // In sq ft or decimals
        public string AreaUnit { get; set; } // "SqFt", "Decimal", "Katha"
        public string PropertyType { get; set; } // "Flat", "Plot", "Floor"
        public string Status { get; set; } // "Available", "Sold", "Pending"
        
        public string Address { get; set; }
        
        // Location Foreign Keys
        public int DivisionId { get; set; }
        public Division? Division { get; set; }
        
        public int DistrictId { get; set; }
        public District? District { get; set; }
        
        public int UpazillaId { get; set; }
        public Upazilla? Upazilla { get; set; }

        // Seller
        public string SellerId { get; set; }
        public ApplicationUser? Seller { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; }

        public ICollection<PropertyImage>? Images { get; set; }
    }

    public class PropertyImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }

    public class SavedProperty
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
