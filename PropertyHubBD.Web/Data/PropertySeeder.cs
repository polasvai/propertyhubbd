using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PropertyHubBD.Web.Data
{
    public static class PropertySeeder
    {
        public static async Task SeedProperties(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if properties already exist
            if (context.Properties.Any())
            {
                return; // DB has been seeded
            }

            // Get Admin User
            var adminUser = await userManager.FindByEmailAsync("superadmin@propertyhubbd.com");
            if (adminUser == null)
            {
                // Admin should have been seeded by AdminSeeder, but handle case if not
                return;
            }

            // Get Locations (Ensure at least some exist)
            var divisions = await context.Divisions.ToListAsync();
            var districts = await context.Districts.ToListAsync();
            
            // If strictly relying on DbInitializer, we might have limited districts. 
            // Let's rely on what's available or default to the first one.
            if (!divisions.Any()) return;

            var properties = new List<Property>();
            var random = new Random();

            string[] propertyTypes = { "Flat", "Plot", "Commercial Space", "Duplex", "Penthouse" };
            string[] statuses = { "Available", "Sold", "Pending" };
            string[] areaUnits = { "SqFt", "Katha", "Decimal" };
            
            // Sample Images (Placeholders)
            string[] sampleImages = {
                "https://images.unsplash.com/photo-1560518883-ce09059eeffa?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1600596542815-60c376663620?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80",
                "https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80"
            };

            for (int i = 1; i <= 50; i++)
            {
                var division = divisions[random.Next(divisions.Count)];
                // Try to find a district in this division, otherwise fallback
                var divisionDistricts = districts.Where(d => d.DivisionId == division.Id).ToList();
                var district = divisionDistricts.Any() ? divisionDistricts[random.Next(divisionDistricts.Count)] : null;
                
                // If no district map, just pick random district or create dummy
                if (district == null && districts.Any()) {
                     district = districts[random.Next(districts.Count)];
                }

                // Create property
                var property = new Property
                {
                    Title = $"{propertyTypes[random.Next(propertyTypes.Length)]} for Sale in {division.Name} - {i}",
                    Description = $"This is a luxurious {propertyTypes[random.Next(propertyTypes.Length)]} located in the heart of {division.Name}. \n\nFeatures:\n- {random.Next(2, 6)} Bedrooms\n- {random.Next(2, 5)} Bathrooms\n- Modern Kitchen\n- Parking Space\n\nCall for more details.",
                    Price = random.Next(1000000, 50000000), // 10 Lakh to 5 Crore
                    AreaSize = random.Next(800, 5000),
                    AreaUnit = areaUnits[random.Next(areaUnits.Length)],
                    PropertyType = propertyTypes[random.Next(propertyTypes.Length)],
                    Status = "Available", // Mostly available for demo
                    Address = $"House #{random.Next(1, 100)}, Road #{random.Next(1, 20)}, Block {((char)('A' + random.Next(0, 6)))}, {division.Name}",
                    DivisionId = division.Id,
                    DistrictId = district?.Id ?? 1, // Fallback
                    UpazillaId = 1, // We assume at least one Upazilla exists or we might need to seed one.
                    SellerId = adminUser.Id,
                    IsApproved = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 60)), // Created in last 60 days
                    Images = new List<PropertyImage>
                    {
                        new PropertyImage { ImageUrl = sampleImages[random.Next(sampleImages.Length)] }
                    }
                };

                properties.Add(property);
            }

            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();
        }
    }
}
