using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyHubBD.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetDistricts/{divisionId}")]
        public async Task<IActionResult> GetDistricts(int divisionId)
        {
            var districts = await _context.Districts
                .Where(d => d.DivisionId == divisionId)
                .OrderBy(d => d.Name)
                .Select(d => new { id = d.Id, name = d.Name })
                .ToListAsync();
            return Ok(districts);
        }

        [HttpGet("GetUpazillas/{districtId}")]
        public async Task<IActionResult> GetUpazillas(int districtId)
        {
            var upazillas = await _context.Upazillas
                .Where(u => u.DistrictId == districtId)
                .OrderBy(u => u.Name)
                .Select(u => new { id = u.Id, name = u.Name })
                .ToListAsync();
            return Ok(upazillas);
        }
    }
}
