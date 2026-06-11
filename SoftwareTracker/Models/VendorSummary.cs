namespace SoftwareTracker.Models;

/// <summary>
/// Lightweight projection of a vendor with related-record counts,
/// used by the vendor list and delete-confirmation pages.
/// </summary>
public class VendorSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int ContactCount { get; set; }
    public int SoftwareTitleCount { get; set; }
    public int LicensePurchaseCount { get; set; }
    public int MaintenanceContractCount { get; set; }
    public int SubscriptionCount { get; set; }
}
