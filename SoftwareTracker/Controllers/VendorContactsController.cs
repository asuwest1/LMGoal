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
    public async Task<IActionResult> Create(VendorContact contact)
    {
        if (ModelState.IsValid)
        {
            if (contact.IsPrimary)
                await ClearPrimaryFlag(contact.VendorId);

            _context.VendorContacts.Add(contact);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Contact '{contact.Name}' added.";
            return RedirectToAction("Details", "Vendors", new { id = contact.VendorId });
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
    public async Task<IActionResult> Edit(int id, VendorContact contact)
    {
        if (id != contact.Id) return NotFound();

        if (ModelState.IsValid)
        {
            if (contact.IsPrimary)
                await ClearPrimaryFlag(contact.VendorId, excludeId: id);

            try
            {
                _context.Update(contact);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Contact '{contact.Name}' updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.VendorContacts.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return RedirectToAction("Details", "Vendors", new { id = contact.VendorId });
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

    private async Task ClearPrimaryFlag(int vendorId, int? excludeId = null)
    {
        var existing = await _context.VendorContacts
            .Where(c => c.VendorId == vendorId && c.IsPrimary && (excludeId == null || c.Id != excludeId))
            .ToListAsync();
        foreach (var c in existing)
            c.IsPrimary = false;
    }
}
