using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyHubBD.Web.Data;
using PropertyHubBD.Web.Models;
using System.Diagnostics;

namespace PropertyHubBD.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int? divisionId, string search)
    {
        var viewModel = new HomeViewModel
        {
            SelectedDivisionId = divisionId,
            SearchTerm = search
        };

        // Fetch Divisions
        viewModel.Divisions = await _context.Divisions
            .Select(d => new DivisionViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Color = d.Color,
                PropertyCount = _context.Properties.Count(p => p.DivisionId == d.Id)
            })
            .ToListAsync();

        // Build Property Query
        var propertyQuery = _context.Properties
            .Include(p => p.Division)
            .Include(p => p.District)
            .Include(p => p.Upazilla)
            .Include(p => p.Images)
            .Where(p => p.Status == "Available" && p.IsApproved);

        if (divisionId.HasValue)
        {
            propertyQuery = propertyQuery.Where(p => p.DivisionId == divisionId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            propertyQuery = propertyQuery.Where(p => 
                p.Title.Contains(search) || 
                p.Description.Contains(search) || 
                p.Address.Contains(search));
        }

        // Fetch Recent/Filtered Properties
        viewModel.Properties = await propertyQuery
            .OrderByDescending(p => p.CreatedAt)
            .Take(9)
            .ToListAsync();

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
