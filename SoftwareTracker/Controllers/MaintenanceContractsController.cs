using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class MaintenanceContractsController : Controller
{
    private readonly ApplicationDbContext _context;

    public MaintenanceContractsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? filter, int? licensePurchaseId)
    {
        var today = DateTime.Today;
        var query = _context.MaintenanceContracts
            .Include(c => c.LicensePurchase).ThenInclude(lp => lp!.SoftwareTitle)
            .Include(c => c.Vendor)
            .AsQueryable();

        if (licensePurchaseId.HasValue)
            query = query.Where(c => c.LicensePurchaseId == licensePurchaseId);

        query = filter switch
        {
            "active" => query.Where(c => c.EndDate >= today),
            "expired" => query.Where(c => c.EndDate < today),
            "expiring" => query.Where(c => c.EndDate >= today && c.EndDate <= today.AddDays(60)),
            _ => query
        };

        ViewBag.Filter = filter;
        ViewBag.LicensePurchaseId = licensePurchaseId;
        ViewBag.LicensePurchases = await _context.LicensePurchases
            .Include(lp => lp.SoftwareTitle)
            .OrderBy(lp => lp.SoftwareTitle!.Name)
            .ToListAsync();

        return View(await query.OrderBy(c => c.EndDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var contract = await _context.MaintenanceContracts
            .Include(c => c.LicensePurchase).ThenInclude(lp => lp!.SoftwareTitle)
            .Include(c => c.Vendor)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null) return NotFound();
        return View(contract);
    }

    public async Task<IActionResult> Create(int? licensePurchaseId)
    {
        await PopulateLicensePurchases(licensePurchaseId);
        await PopulateVendors();
        var contract = new MaintenanceContract
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        if (licensePurchaseId.HasValue) contract.LicensePurchaseId = licensePurchaseId.Value;
        return View(contract);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaintenanceContract contract)
    {
        if (contract.EndDate <= contract.StartDate)
            ModelState.AddModelError("EndDate", "End date must be after start date.");

        if (ModelState.IsValid)
        {
            _context.MaintenanceContracts.Add(contract);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Maintenance contract created successfully.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateLicensePurchases(contract.LicensePurchaseId);
        await PopulateVendors(contract.VendorId);
        return View(contract);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var contract = await _context.MaintenanceContracts.FindAsync(id);
        if (contract == null) return NotFound();
        await PopulateLicensePurchases(contract.LicensePurchaseId);
        await PopulateVendors(contract.VendorId);
        return View(contract);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MaintenanceContract contract)
    {
        if (id != contract.Id) return NotFound();

        if (contract.EndDate <= contract.StartDate)
            ModelState.AddModelError("EndDate", "End date must be after start date.");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(contract);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Maintenance contract updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.MaintenanceContracts.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        await PopulateLicensePurchases(contract.LicensePurchaseId);
        await PopulateVendors(contract.VendorId);
        return View(contract);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var contract = await _context.MaintenanceContracts
            .Include(c => c.LicensePurchase).ThenInclude(lp => lp!.SoftwareTitle)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contract == null) return NotFound();
        return View(contract);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contract = await _context.MaintenanceContracts.FindAsync(id);
        if (contract != null)
        {
            _context.MaintenanceContracts.Remove(contract);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Maintenance contract deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLicensePurchases(int? selectedId = null)
    {
        var purchases = await _context.LicensePurchases
            .Include(lp => lp.SoftwareTitle)
            .OrderBy(lp => lp.SoftwareTitle!.Name)
            .ThenBy(lp => lp.PurchaseDate)
            .ToListAsync();

        ViewBag.LicensePurchaseId = new SelectList(
            purchases.Select(lp => new
            {
                lp.Id,
                Display = $"{lp.SoftwareTitle?.Name} — purchased {lp.PurchaseDate:yyyy-MM-dd} (qty: {lp.Quantity})"
            }),
            "Id", "Display", selectedId);
    }

    private async Task PopulateVendors(int? selectedId = null)
    {
        ViewBag.VendorId = new SelectList(
            await _context.Vendors.OrderBy(v => v.Name).ToListAsync(),
            "Id", "Name", selectedId);
    }
}
