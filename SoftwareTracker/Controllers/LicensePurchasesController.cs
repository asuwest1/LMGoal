using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class LicensePurchasesController : Controller
{
    private readonly ApplicationDbContext _context;

    public LicensePurchasesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? softwareTitleId, string? search)
    {
        var query = _context.LicensePurchases
            .Include(lp => lp.SoftwareTitle)
            .Include(lp => lp.MaintenanceContracts)
            .AsQueryable();

        if (softwareTitleId.HasValue)
            query = query.Where(lp => lp.SoftwareTitleId == softwareTitleId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(lp => lp.SoftwareTitle!.Name.Contains(search) ||
                                      (lp.LicenseKey != null && lp.LicenseKey.Contains(search)) ||
                                      (lp.OrderNumber != null && lp.OrderNumber.Contains(search)));

        ViewBag.Search = search;
        ViewBag.SoftwareTitleId = softwareTitleId;
        ViewBag.SoftwareTitles = await _context.SoftwareTitles.OrderBy(s => s.Name).ToListAsync();

        return View(await query.OrderByDescending(lp => lp.PurchaseDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var lp = await _context.LicensePurchases
            .Include(l => l.SoftwareTitle)
            .Include(l => l.MaintenanceContracts)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (lp == null) return NotFound();
        return View(lp);
    }

    public async Task<IActionResult> Create(int? softwareTitleId)
    {
        await PopulateSoftwareTitles(softwareTitleId);
        var lp = new LicensePurchase { PurchaseDate = DateTime.Today };
        if (softwareTitleId.HasValue) lp.SoftwareTitleId = softwareTitleId.Value;
        return View(lp);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LicensePurchase licensePurchase)
    {
        if (ModelState.IsValid)
        {
            _context.LicensePurchases.Add(licensePurchase);
            await _context.SaveChangesAsync();
            TempData["Success"] = "License purchase recorded successfully.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateSoftwareTitles(licensePurchase.SoftwareTitleId);
        return View(licensePurchase);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var lp = await _context.LicensePurchases.FindAsync(id);
        if (lp == null) return NotFound();
        await PopulateSoftwareTitles(lp.SoftwareTitleId);
        return View(lp);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LicensePurchase licensePurchase)
    {
        if (id != licensePurchase.Id) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(licensePurchase);
                await _context.SaveChangesAsync();
                TempData["Success"] = "License purchase updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.LicensePurchases.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        await PopulateSoftwareTitles(licensePurchase.SoftwareTitleId);
        return View(licensePurchase);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var lp = await _context.LicensePurchases
            .Include(l => l.SoftwareTitle)
            .Include(l => l.MaintenanceContracts)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (lp == null) return NotFound();
        return View(lp);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lp = await _context.LicensePurchases.FindAsync(id);
        if (lp != null)
        {
            _context.LicensePurchases.Remove(lp);
            await _context.SaveChangesAsync();
            TempData["Success"] = "License purchase deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSoftwareTitles(int? selectedId = null)
    {
        ViewBag.SoftwareTitleId = new SelectList(
            await _context.SoftwareTitles.OrderBy(s => s.Name).ToListAsync(),
            "Id", "Name", selectedId);
    }
}
