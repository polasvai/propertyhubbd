using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using PropertyHubBD.Web.Models;
using PropertyHubBD.Web.Filters;

namespace PropertyHubBD.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task SetCurrentUserInViewBag()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser;
            ViewBag.IsSuperAdmin = currentUser?.IsSuperAdmin ?? false;
        }

        public async Task<IActionResult> Index()
        {
            await SetCurrentUserInViewBag();
            
            var recentProperties = await _context.Properties
                .Include(p => p.Seller)
                .Include(p => p.Division)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentUsers = await _userManager.Users
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalProperties = await _context.Properties.CountAsync(),
                PendingProperties = await _context.Properties.CountAsync(p => !p.IsApproved),
                TotalDivisions = await _context.Divisions.CountAsync(),
                RecentProperties = recentProperties,
                RecentUsers = recentUsers
            };
            return View(model);
        }

        public async Task<IActionResult> Properties()
        {
            await SetCurrentUserInViewBag();
            
            var properties = await _context.Properties
                .Include(p => p.Seller)
                .Include(p => p.Division)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(properties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuperAdminAuthorize]
        public async Task<IActionResult> ApproveProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                property.IsApproved = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Property approved successfully!";
            }
            else
            {
                TempData["Error"] = "Property not found!";
            }
            return RedirectToAction(nameof(Properties));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuperAdminAuthorize]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Property deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Property not found!";
            }
            return RedirectToAction(nameof(Properties));
        }

        public async Task<IActionResult> Users()
        {
            await SetCurrentUserInViewBag();
            
            var users = await _userManager.Users
                .OrderByDescending(u => u.Id)
                .ToListAsync();
            return View(users);
        }

        // CREATE PROPERTY - GET
        [HttpGet]
        [CanAddPropertyAuthorize]
        public async Task<IActionResult> Create()
        {
            await SetCurrentUserInViewBag();
            await LoadLocationData();
            return View();
        }

        // CREATE PROPERTY - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanAddPropertyAuthorize]
        public async Task<IActionResult> Create(Property property)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                
                // Set the seller to current user
                property.SellerId = currentUser.Id;
                property.CreatedAt = DateTime.UtcNow;
                
                // Auto-approve if user is Admin or SuperAdmin
                if (currentUser.UserType == "Admin")
                {
                    property.IsApproved = true;
                }
                else
                {
                    property.IsApproved = false;
                }
                
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Property created successfully!";
                return RedirectToAction(nameof(Properties));
            }
            
            await SetCurrentUserInViewBag();
            await LoadLocationData();
            return View(property);
        }

        // EDIT PROPERTY - GET
        [HttpGet]
        [CanEditPropertyAuthorize]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazilla)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (property == null)
            {
                TempData["Error"] = "Property not found!";
                return RedirectToAction(nameof(Properties));
            }
            
            await SetCurrentUserInViewBag();
            await LoadLocationData();
            return View(property);
        }

        // EDIT PROPERTY - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanEditPropertyAuthorize]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (id != property.Id)
            {
                TempData["Error"] = "Invalid property ID!";
                return RedirectToAction(nameof(Properties));
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProperty = await _context.Properties.FindAsync(id);
                    if (existingProperty == null)
                    {
                        TempData["Error"] = "Property not found!";
                        return RedirectToAction(nameof(Properties));
                    }
                    
                    // Update properties
                    existingProperty.Title = property.Title;
                    existingProperty.Description = property.Description;
                    existingProperty.Price = property.Price;
                    existingProperty.AreaSize = property.AreaSize;
                    existingProperty.AreaUnit = property.AreaUnit;
                    existingProperty.PropertyType = property.PropertyType;
                    existingProperty.Status = property.Status;
                    existingProperty.Address = property.Address;
                    existingProperty.DivisionId = property.DivisionId;
                    existingProperty.DistrictId = property.DistrictId;
                    existingProperty.UpazillaId = property.UpazillaId;
                    
                    _context.Update(existingProperty);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Property updated successfully!";
                    return RedirectToAction(nameof(Properties));
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "An error occurred while updating the property.";
                }
            }
            
            await SetCurrentUserInViewBag();
            await LoadLocationData();
            return View(property);
        }

        // Helper method to load location data for dropdowns
        private async Task LoadLocationData()
        {
            ViewBag.Divisions = await _context.Divisions.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Districts = await _context.Districts.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Upazillas = await _context.Upazillas.OrderBy(u => u.Name).ToListAsync();
        }
    }
}
