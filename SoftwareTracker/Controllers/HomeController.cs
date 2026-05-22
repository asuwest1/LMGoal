using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var soonThreshold = today.AddDays(60);

        var vm = new DashboardViewModel
        {
            TotalSoftwareTitles = await _context.SoftwareTitles.CountAsync(),
            TotalLicensePurchases = await _context.LicensePurchases.CountAsync(),
            TotalMaintenanceContracts = await _context.MaintenanceContracts.CountAsync(),
            TotalSubscriptions = await _context.Subscriptions.CountAsync(),

            ActiveMaintenanceContracts = await _context.MaintenanceContracts.CountAsync(c => c.EndDate >= today),
            ActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.EndDate >= today),
            ExpiredMaintenanceContracts = await _context.MaintenanceContracts.CountAsync(c => c.EndDate < today),
            ExpiredSubscriptions = await _context.Subscriptions.CountAsync(s => s.EndDate < today),

            ExpiringSoonContracts = await _context.MaintenanceContracts
                .Include(c => c.LicensePurchase).ThenInclude(lp => lp!.SoftwareTitle)
                .Where(c => c.EndDate >= today && c.EndDate <= soonThreshold)
                .OrderBy(c => c.EndDate)
                .ToListAsync(),

            ExpiringSoonSubscriptions = await _context.Subscriptions
                .Include(s => s.SoftwareTitle)
                .Where(s => s.EndDate >= today && s.EndDate <= soonThreshold)
                .OrderBy(s => s.EndDate)
                .ToListAsync(),

            RecentlyExpiredContracts = await _context.MaintenanceContracts
                .Include(c => c.LicensePurchase).ThenInclude(lp => lp!.SoftwareTitle)
                .Where(c => c.EndDate < today && c.EndDate >= today.AddDays(-30))
                .OrderByDescending(c => c.EndDate)
                .ToListAsync(),

            RecentlyExpiredSubscriptions = await _context.Subscriptions
                .Include(s => s.SoftwareTitle)
                .Where(s => s.EndDate < today && s.EndDate >= today.AddDays(-30))
                .OrderByDescending(s => s.EndDate)
                .ToListAsync(),

            TotalAnnualSubscriptionCost = await _context.Subscriptions
                .Where(s => s.EndDate >= today && s.CostPerPeriod != null)
                .SumAsync(s => s.BillingPeriod == BillingPeriod.Monthly ? s.CostPerPeriod * 12 :
                               s.BillingPeriod == BillingPeriod.Quarterly ? s.CostPerPeriod * 4 :
                               s.BillingPeriod == BillingPeriod.SemiAnnual ? s.CostPerPeriod * 2 :
                               s.CostPerPeriod),

            TotalAnnualMaintenanceCost = await _context.MaintenanceContracts
                .Where(c => c.EndDate >= today && c.AnnualCost != null)
                .SumAsync(c => c.AnnualCost)
        };

        return View(vm);
    }

    public IActionResult Error()
    {
        return View();
    }
}
