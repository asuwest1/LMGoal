using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers;

public class VendorContactsController : Controller
{
    private readonly ApplicationDbContext _context;

    public VendorContactsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Create(int vendorId)
    {
        var vendor = await _context.Vendors.FindAsync(vendorId);
        if (vendor == null) return NotFound();

        var contact = new VendorContact { VendorId = vendorId };
        ViewBag.VendorName = vendor.Name;
        return View(contact);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,VendorId,Name,Title,Email,Phone,IsPrimary,Notes")] VendorContact contact)
    {
        if (!await _context.Vendors.AnyAsync(v => v.Id == contact.VendorId))
            ModelState.AddModelError(string.Empty, "The selected vendor no longer exists.");

        if (ModelState.IsValid)
        {
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                if (contact.IsPrimary)
                    await ClearPrimaryFlag(contact.VendorId);

                _context.VendorContacts.Add(contact);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["Success"] = $"Contact '{contact.Name}' added.";
                return RedirectToAction("Details", "Vendors", new { id = contact.VendorId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "The contact could not be saved. Another contact may have just been marked as primary — please try again.");
            }
        }

        var vendor = await _context.Vendors.FindAsync(contact.VendorId);
        ViewBag.VendorName = vendor?.Name;
        return View(contact);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var contact = await _context.VendorContacts
            .Include(c => c.Vendor)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        ViewBag.VendorName = contact.Vendor?.Name;
        return View(contact);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,VendorId,Name,Title,Email,Phone,IsPrimary,Notes")] VendorContact contact)
    {
        if (id != contact.Id) return NotFound();

        if (!await _context.Vendors.AnyAsync(v => v.Id == contact.VendorId))
            ModelState.AddModelError(string.Empty, "The selected vendor no longer exists.");

        if (ModelState.IsValid)
        {
            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                if (contact.IsPrimary)
                    await ClearPrimaryFlag(contact.VendorId, excludeId: id);

                _context.Update(contact);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["Success"] = $"Contact '{contact.Name}' updated.";
                return RedirectToAction("Details", "Vendors", new { id = contact.VendorId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.VendorContacts.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "The contact could not be saved. Another contact may have just been marked as primary — please try again.");
            }
        }

        var vendor = await _context.Vendors.FindAsync(contact.VendorId);
        ViewBag.VendorName = vendor?.Name;
        return View(contact);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var contact = await _context.VendorContacts
            .Include(c => c.Vendor)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();
        return View(contact);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contact = await _context.VendorContacts.FindAsync(id);
        if (contact != null)
        {
            int vendorId = contact.VendorId;
            _context.VendorContacts.Remove(contact);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Contact '{contact.Name}' deleted.";
            return RedirectToAction("Details", "Vendors", new { id = vendorId });
        }
        return RedirectToAction("Index", "Vendors");
    }

    private Task<int> ClearPrimaryFlag(int vendorId, int? excludeId = null)
    {
        return _context.VendorContacts
            .Where(c => c.VendorId == vendorId && c.IsPrimary && (excludeId == null || c.Id != excludeId))
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsPrimary, false));
    }
}
