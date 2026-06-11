using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class SubscriptionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SubscriptionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? filter, int? softwareTitleId)
    {
        var today = DateTime.Today;
        var query = _context.Subscriptions
            .Include(s => s.SoftwareTitle)
            .Include(s => s.Vendor)
            .AsQueryable();

        if (softwareTitleId.HasValue)
            query = query.Where(s => s.SoftwareTitleId == softwareTitleId);

        query = filter switch
        {
            "active" => query.Where(s => s.EndDate >= today),
            "expired" => query.Where(s => s.EndDate < today),
            "expiring" => query.Where(s => s.EndDate >= today && s.EndDate <= today.AddDays(60)),
            _ => query
        };

        ViewBag.Filter = filter;
        ViewBag.SoftwareTitleId = softwareTitleId;
        ViewBag.SoftwareTitles = await _context.SoftwareTitles.OrderBy(s => s.Name).ToListAsync();

        return View(await query.OrderBy(s => s.EndDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var sub = await _context.Subscriptions
            .Include(s => s.SoftwareTitle)
            .Include(s => s.Vendor)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null) return NotFound();
        return View(sub);
    }

    public async Task<IActionResult> Create(int? softwareTitleId, int? vendorId)
    {
        await PopulateSoftwareTitles(softwareTitleId);
        await PopulateVendors(vendorId);
        var sub = new Subscription
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
            AutoRenews = true
        };
        if (softwareTitleId.HasValue) sub.SoftwareTitleId = softwareTitleId.Value;
        if (vendorId.HasValue) sub.VendorId = vendorId.Value;
        return View(sub);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Subscription subscription)
    {
        if (subscription.EndDate <= subscription.StartDate)
            ModelState.AddModelError("EndDate", "End/renewal date must be after start date.");

        await ValidateVendorExists(subscription.VendorId);
        if (ModelState.IsValid)
        {
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription created successfully.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateSoftwareTitles(subscription.SoftwareTitleId);
        await PopulateVendors(subscription.VendorId);
        return View(subscription);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var sub = await _context.Subscriptions.FindAsync(id);
        if (sub == null) return NotFound();
        await PopulateSoftwareTitles(sub.SoftwareTitleId);
        await PopulateVendors(sub.VendorId);
        return View(sub);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Subscription subscription)
    {
        if (id != subscription.Id) return NotFound();

        if (subscription.EndDate <= subscription.StartDate)
            ModelState.AddModelError("EndDate", "End/renewal date must be after start date.");

        await ValidateVendorExists(subscription.VendorId);
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(subscription);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subscription updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Subscriptions.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        await PopulateSoftwareTitles(subscription.SoftwareTitleId);
        await PopulateVendors(subscription.VendorId);
        return View(subscription);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var sub = await _context.Subscriptions
            .Include(s => s.SoftwareTitle)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null) return NotFound();
        return View(sub);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sub = await _context.Subscriptions.FindAsync(id);
        if (sub != null)
        {
            _context.Subscriptions.Remove(sub);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Subscription deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSoftwareTitles(int? selectedId = null)
    {
        ViewBag.SoftwareTitleId = new SelectList(
            await _context.SoftwareTitles.OrderBy(s => s.Name).ToListAsync(),
            "Id", "Name", selectedId);
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
