using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Upazilla> Upazillas { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<SavedProperty> SavedProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Disable cascading deletes for Location relationships to avoid cycles
            builder.Entity<Property>()
                .HasOne(p => p.Division)
                .WithMany()
                .HasForeignKey(p => p.DivisionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Property>()
                .HasOne(p => p.District)
                .WithMany()
                .HasForeignKey(p => p.DistrictId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Property>()
                .HasOne(p => p.Upazilla)
                .WithMany()
                .HasForeignKey(p => p.UpazillaId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<District>()
                .HasOne(d => d.Division)
                .WithMany(d => d.Districts)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Upazilla>()
                .HasOne(u => u.District)
                .WithMany(d => d.Upazillas)
                .HasForeignKey(u => u.DistrictId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Property>()
                .HasOne(p => p.Seller)
                .WithMany()
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SavedProperty>()
                .HasOne(sp => sp.Property)
                .WithMany()
                .HasForeignKey(sp => sp.PropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SavedProperty>()
                .HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
