using Microsoft.AspNetCore.Identity;
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminUser(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var adminEmail = "superadmin@propertyhubbd.com";
            var adminPassword = "Admin@123";

            try
            {
                logger.LogInformation("Starting admin user seeding...");
                
                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

                if (existingAdmin == null)
                {
                    logger.LogInformation("Admin user not found. Creating new admin user...");
                    
                    // Create new admin user
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Super Admin",
                        UserType = "Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        logger.LogInformation("✅ Admin user created successfully: {Email}", adminEmail);
                        Console.WriteLine($"✅ Admin user created: {adminEmail}");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogError("❌ Failed to create admin user: {Errors}", errors);
                        Console.WriteLine($"❌ Failed to create admin user: {errors}");
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists. Updating to ensure Admin role...");
                    
                    // Update existing user to Admin
                    existingAdmin.UserType = "Admin";
                    existingAdmin.FullName = "Super Admin";
                    existingAdmin.EmailConfirmed = true;
                    
                    var result = await userManager.UpdateAsync(existingAdmin);
                    
                    if (result.Succeeded)
                    {
                        logger.LogInformation("✅ User updated to Admin: {Email}", adminEmail);
                        Console.WriteLine($"✅ User updated to Admin: {adminEmail}");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogError("❌ Failed to update user: {Errors}", errors);
                        Console.WriteLine($"❌ Failed to update user: {errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Exception occurred while seeding admin user");
                Console.WriteLine($"❌ Exception: {ex.Message}");
                throw;
            }
        }
    }
}
