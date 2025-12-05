using PropertyHubBD.Web.Models;
using System.Linq;

namespace PropertyHubBD.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any divisions.
            if (context.Divisions.Any())
            {
                return;   // DB has been seeded
            }

            var divisions = new Division[]
            {
                new Division{Name="Dhaka", Color="#FF5733"},
                new Division{Name="Chittagong", Color="#33FF57"},
                new Division{Name="Rajshahi", Color="#3357FF"},
                new Division{Name="Khulna", Color="#F333FF"},
                new Division{Name="Barisal", Color="#FF33A1"},
                new Division{Name="Sylhet", Color="#33FFF3"},
                new Division{Name="Rangpur", Color="#F3FF33"},
                new Division{Name="Mymensingh", Color="#FF8C33"}
            };
            foreach (Division d in divisions)
            {
                context.Divisions.Add(d);
            }
            context.SaveChanges();

            // Add some districts for Dhaka
            var dhakaId = divisions.Single(d => d.Name == "Dhaka").Id;
            var districts = new District[]
            {
                new District{Name="Dhaka", DivisionId=dhakaId},
                new District{Name="Gazipur", DivisionId=dhakaId},
                new District{Name="Narayanganj", DivisionId=dhakaId}
            };
            foreach (District d in districts)
            {
                context.Districts.Add(d);
            }
            context.SaveChanges();
        }
    }
}
