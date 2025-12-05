using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using PropertyHubBD.Web.Models;

namespace PropertyHubBD.Web.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Division(int id)
        {
            var division = await _context.Divisions
                .Include(d => d.Districts)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (division == null)
            {
                return NotFound();
            }

            var properties = await _context.Properties
                .Where(p => p.DivisionId == id)
                .Include(p => p.Division)
                .Include(p => p.District)
                .ToListAsync();

            var viewModel = new DivisionPropertiesViewModel
            {
                Division = division,
                Properties = properties
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Search(string q)
        {
            var query = _context.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(p => p.Title.Contains(q) || p.Description.Contains(q) || p.Address.Contains(q));
            }

            var properties = await query
                .Include(p => p.Division)
                .Include(p => p.District)
                .ToListAsync();

            return View("Index", properties);
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Division)
                .Include(p => p.District)
                .Include(p => p.Upazilla)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }
    }
}
