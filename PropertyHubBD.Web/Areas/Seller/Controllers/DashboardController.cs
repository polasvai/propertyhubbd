using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        private async Task<bool> IsAuthorized()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.UserType == "Seller" || user?.UserType == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!await IsAuthorized())
                return Forbid();

            var user = await _userManager.GetUserAsync(User);
            var properties = await _context.Properties
                .Where(p => p.SellerId == user.Id)
                .Include(p => p.Division)
                .Include(p => p.District)
                .ToListAsync();
            return View(properties);
        }

        public async Task<IActionResult> Create()
        {
            if (!await IsAuthorized())
                return Forbid();

            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["UpazillaId"] = new SelectList(_context.Upazillas, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Property property, List<IFormFile> images)
        {
            if (!await IsAuthorized())
                return Forbid();

            var user = await _userManager.GetUserAsync(User);
            property.SellerId = user.Id;
            property.CreatedAt = DateTime.UtcNow;
            property.IsApproved = false;

            if (string.IsNullOrEmpty(property.Title)) ModelState.AddModelError("Title", "Title is required");

            // Remove SellerId/Seller from validation since valid values are set in controller
            ModelState.Remove("SellerId");
            ModelState.Remove("Seller");
            ModelState.Remove("Images");

            if (ModelState.IsValid)
            {
                _context.Add(property);
                await _context.SaveChangesAsync();

                if (images != null && images.Any())
                {
                    string path = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    foreach (var img in images)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }
                        _context.PropertyImages.Add(new PropertyImage { PropertyId = property.Id, ImageUrl = "/uploads/" + fileName });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", property.DivisionId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", property.DistrictId);
            ViewData["UpazillaId"] = new SelectList(_context.Upazillas, "Id", "Name", property.UpazillaId);
            return View(property);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!await IsAuthorized())
                return Forbid();

            if (id == null) return NotFound();

            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (property.SellerId != user.Id && user.UserType != "Admin") return Forbid();

            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", property.DivisionId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", property.DistrictId);
            ViewData["UpazillaId"] = new SelectList(_context.Upazillas, "Id", "Name", property.UpazillaId);
            return View(property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property)
        {
            if (!await IsAuthorized())
                return Forbid();

            if (id != property.Id) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var existing = await _context.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existing == null) return NotFound();
            if (existing.SellerId != user.Id && user.UserType != "Admin") return Forbid();

            property.SellerId = existing.SellerId;
            property.CreatedAt = existing.CreatedAt;
            property.IsApproved = false;

            property.IsApproved = false;

            // Remove SellerId/Seller from validation
            ModelState.Remove("SellerId");
            ModelState.Remove("Seller");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Properties.Any(e => e.Id == property.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", property.DivisionId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", property.DistrictId);
            ViewData["UpazillaId"] = new SelectList(_context.Upazillas, "Id", "Name", property.UpazillaId);
            return View(property);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!await IsAuthorized())
                return Forbid();

            if (id == null) return NotFound();

            var property = await _context.Properties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (property.SellerId != user.Id && user.UserType != "Admin") return Forbid();

            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await IsAuthorized())
                return Forbid();

            var property = await _context.Properties.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (property.SellerId != user.Id && user.UserType != "Admin") return Forbid();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
