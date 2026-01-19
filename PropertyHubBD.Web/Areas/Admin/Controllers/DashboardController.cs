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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.UserType != "Admin") return Forbid();
            
            var users = await _userManager.Users
                .OrderByDescending(u => u.Id)
                .ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SuperAdminAuthorize] // Optional: keep it restrictive or allow regular Admins? 
        // User asked "admin can delete", so maybe just AdminAuthorize (implied by class).
        // But deleting users is dangerous. Let's keep it class level AdminAuthorize.
        public async Task<IActionResult> DeleteUser(string id)
        {
             var user = await _userManager.FindByIdAsync(id);
             if (user != null)
             {
                 await _userManager.DeleteAsync(user);
                 TempData["Success"] = "User deleted successfully!";
             }
             else
             {
                 TempData["Error"] = "User not found!";
             }
             return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            await SetCurrentUserInViewBag();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found!";
                return RedirectToAction(nameof(Users));
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, ApplicationUser updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = updatedUser.FullName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.UserType = updatedUser.UserType;
            // Optionally update other fields

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User updated successfully!";
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await SetCurrentUserInViewBag();
            return View(user);
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

        // EDIT PROPERTY - GET
        [HttpGet]
        [CanEditPropertyAuthorize]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazilla)
                .Include(p => p.Images)
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

        private async Task<List<PropertyImage>> ProcessUploadedImages(List<IFormFile> photos)
        {
             var images = new List<PropertyImage>();
             if (photos != null && photos.Count > 0)
             {
                 string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "properties");
                 if (!Directory.Exists(uploadFolder))
                 {
                     Directory.CreateDirectory(uploadFolder);
                 }

                 foreach (var photo in photos)
                 {
                     if (photo.Length > 0)
                     {
                         string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                         string filePath = Path.Combine(uploadFolder, uniqueFileName);
                         
                         using (var fileStream = new FileStream(filePath, FileMode.Create))
                         {
                             await photo.CopyToAsync(fileStream);
                         }

                         images.Add(new PropertyImage { ImageUrl = "/uploads/properties/" + uniqueFileName });
                     }
                 }
             }
             return images;
        }

        // CREATE PROPERTY - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanAddPropertyAuthorize]
        public async Task<IActionResult> Create(Property property, List<IFormFile> photos)
        {
            // Remove SellerId/Seller from validation since valid values are set in controller
            ModelState.Remove("SellerId");
            ModelState.Remove("Seller");

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
                
                // Handle Images
                var uploadedImages = await ProcessUploadedImages(photos);
                if (uploadedImages.Any())
                {
                    property.Images = uploadedImages;
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

        // EDIT PROPERTY - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanEditPropertyAuthorize]
        public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> photos)
        {
            if (id != property.Id)
            {
                TempData["Error"] = "Invalid property ID!";
                return RedirectToAction(nameof(Properties));
            }
            
            if (ModelState.IsValid)
            {
                // Remove SellerId/Seller from validation
                ModelState.Remove("SellerId");
                ModelState.Remove("Seller");

                try
                {
                    var existingProperty = await _context.Properties
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(p => p.Id == id);

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
                    
                    // Handle New Images
                    var uploadedImages = await ProcessUploadedImages(photos);
                    if (uploadedImages.Any())
                    {
                        if (existingProperty.Images == null) 
                            existingProperty.Images = new List<PropertyImage>();
                            
                        // Append new images
                        foreach(var img in uploadedImages)
                        {
                            existingProperty.Images.Add(img);
                        }
                    }

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
