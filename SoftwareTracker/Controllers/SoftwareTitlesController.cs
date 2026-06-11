using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class SoftwareTitlesController : Controller
{
    private readonly ApplicationDbContext _context;

    public SoftwareTitlesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, string? category)
    {
        var query = _context.SoftwareTitles
            .Include(s => s.Vendor)
            .Include(s => s.LicensePurchases)
            .Include(s => s.Subscriptions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search) ||
                                     (s.Vendor != null && s.Vendor.Name.Contains(search)));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(s => s.Category == category);

        ViewBag.Search = search;
        ViewBag.Category = category;
        ViewBag.Categories = await _context.SoftwareTitles
            .Where(s => s.Category != null)
            .Select(s => s.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return View(await query.OrderBy(s => s.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var softwareTitle = await _context.SoftwareTitles
            .Include(s => s.Vendor)
            .Include(s => s.LicensePurchases)
                .ThenInclude(lp => lp.MaintenanceContracts)
            .Include(s => s.Subscriptions)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (softwareTitle == null) return NotFound();
        return View(softwareTitle);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateVendors();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SoftwareTitle softwareTitle)
    {
        await ValidateVendorExists(softwareTitle.VendorId);
        if (ModelState.IsValid)
        {
            _context.SoftwareTitles.Add(softwareTitle);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Software title '{softwareTitle.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateVendors(softwareTitle.VendorId);
        return View(softwareTitle);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var softwareTitle = await _context.SoftwareTitles.FindAsync(id);
        if (softwareTitle == null) return NotFound();
        await PopulateVendors(softwareTitle.VendorId);
        return View(softwareTitle);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SoftwareTitle softwareTitle)
    {
        if (id != softwareTitle.Id) return NotFound();

        await ValidateVendorExists(softwareTitle.VendorId);
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(softwareTitle);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Software title '{softwareTitle.Name}' updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SoftwareTitles.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        await PopulateVendors(softwareTitle.VendorId);
        return View(softwareTitle);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var softwareTitle = await _context.SoftwareTitles
            .Include(s => s.Vendor)
            .Include(s => s.LicensePurchases)
            .Include(s => s.Subscriptions)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (softwareTitle == null) return NotFound();
        return View(softwareTitle);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var softwareTitle = await _context.SoftwareTitles.FindAsync(id);
        if (softwareTitle != null)
        {
            _context.SoftwareTitles.Remove(softwareTitle);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Software title '{softwareTitle.Name}' deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateVendors(int? selectedId = null)
    {
        ViewBag.VendorId = new SelectList(
            await _context.Vendors.OrderBy(v => v.Name).ToListAsync(),
            "Id", "Name", selectedId);
    }

    private async Task ValidateVendorExists(int? vendorId)
    {
        if (vendorId.HasValue && !await _context.Vendors.AnyAsync(v => v.Id == vendorId))
            ModelState.AddModelError("VendorId", "The selected vendor no longer exists.");
    }
}
