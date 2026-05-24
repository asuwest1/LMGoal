using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class VendorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public VendorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Vendors
            .Include(v => v.Contacts)
            .Include(v => v.SoftwareTitles)
            .Include(v => v.LicensePurchases)
            .Include(v => v.MaintenanceContracts)
            .Include(v => v.Subscriptions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(v => v.Name.Contains(search) ||
                                     (v.Email != null && v.Email.Contains(search)) ||
                                     (v.Website != null && v.Website.Contains(search)));

        ViewBag.Search = search;
        return View(await query.OrderBy(v => v.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vendor = await _context.Vendors
            .Include(v => v.Contacts)
            .Include(v => v.SoftwareTitles)
            .Include(v => v.LicensePurchases)
                .ThenInclude(lp => lp.SoftwareTitle)
            .Include(v => v.LicensePurchases)
                .ThenInclude(lp => lp.MaintenanceContracts)
            .Include(v => v.MaintenanceContracts)
                .ThenInclude(mc => mc.LicensePurchase)
                    .ThenInclude(lp => lp!.SoftwareTitle)
            .Include(v => v.Subscriptions)
                .ThenInclude(s => s.SoftwareTitle)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vendor == null) return NotFound();
        return View(vendor);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vendor vendor)
    {
        if (ModelState.IsValid)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Vendor '{vendor.Name}' created successfully.";
            return RedirectToAction(nameof(Details), new { id = vendor.Id });
        }
        return View(vendor);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return NotFound();
        return View(vendor);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vendor vendor)
    {
        if (id != vendor.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vendor);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Vendor '{vendor.Name}' updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Vendors.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Details), new { id = vendor.Id });
        }
        return View(vendor);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var vendor = await _context.Vendors
            .Include(v => v.Contacts)
            .Include(v => v.SoftwareTitles)
            .Include(v => v.LicensePurchases)
            .Include(v => v.MaintenanceContracts)
            .Include(v => v.Subscriptions)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (vendor == null) return NotFound();
        return View(vendor);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor != null)
        {
            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Vendor '{vendor.Name}' deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
