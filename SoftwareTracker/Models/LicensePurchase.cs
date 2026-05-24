using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareTracker.Models;

public enum LicenseType
{
    Perpetual,
    [Display(Name = "Subscription-Based")]
    SubscriptionBased,
    [Display(Name = "Per User")]
    PerUser,
    [Display(Name = "Per Device")]
    PerDevice,
    [Display(Name = "Site License")]
    SiteLicense,
    Other
}

public class LicensePurchase
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Software Title")]
    public int SoftwareTitleId { get; set; }
    public SoftwareTitle? SoftwareTitle { get; set; }

    [Display(Name = "Vendor")]
    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    [Required]
    [Display(Name = "Purchase Date")]
    [DataType(DataType.Date)]
    public DateTime PurchaseDate { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "License Quantity")]
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Unit Cost")]
    [DataType(DataType.Currency)]
    public decimal? UnitCost { get; set; }

    [StringLength(500)]
    [Display(Name = "License Key / Serial")]
    public string? LicenseKey { get; set; }

    [Display(Name = "License Type")]
    public LicenseType LicenseType { get; set; } = LicenseType.Perpetual;

    [StringLength(200)]
    [Display(Name = "Order / PO Number")]
    public string? OrderNumber { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public ICollection<MaintenanceContract> MaintenanceContracts { get; set; } = new List<MaintenanceContract>();

    [NotMapped]
    [Display(Name = "Total Cost")]
    public decimal? TotalCost => UnitCost.HasValue ? UnitCost * Quantity : null;
}
