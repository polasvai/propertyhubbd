using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var savedProperties = await _context.SavedProperties
                .Where(sp => sp.UserId == user.Id)
                .Include(sp => sp.Property)
                .ThenInclude(p => p.Division)
                .Include(sp => sp.Property)
                .ThenInclude(p => p.District)
                .Select(sp => sp.Property)
                .ToListAsync();

            return View(savedProperties);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProperty(int propertyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (!_context.SavedProperties.Any(sp => sp.UserId == user.Id && sp.PropertyId == propertyId))
            {
                _context.SavedProperties.Add(new SavedProperty { UserId = user.Id, PropertyId = propertyId });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveProperty(int propertyId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var saved = await _context.SavedProperties
                .FirstOrDefaultAsync(sp => sp.UserId == user.Id && sp.PropertyId == propertyId);

            if (saved != null)
            {
                _context.SavedProperties.Remove(saved);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
