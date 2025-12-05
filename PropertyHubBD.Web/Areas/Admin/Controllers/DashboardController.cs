using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Should check for Admin role ideally
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
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalProperties = await _context.Properties.CountAsync(),
                PendingProperties = await _context.Properties.CountAsync(p => !p.IsApproved),
                TotalDivisions = await _context.Divisions.CountAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Properties()
        {
            var properties = await _context.Properties
                .Include(p => p.Seller)
                .Include(p => p.Division)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(properties);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                property.IsApproved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Properties));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Properties));
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}
