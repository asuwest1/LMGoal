using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareTracker.Models;

public class MaintenanceContract
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "License Purchase")]
    public int LicensePurchaseId { get; set; }
    public LicensePurchase? LicensePurchase { get; set; }

    [Display(Name = "Vendor")]
    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    [StringLength(200)]
    [Display(Name = "Contract Number")]
    public string? ContractNumber { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Annual Cost")]
    [DataType(DataType.Currency)]
    public decimal? AnnualCost { get; set; }

    [StringLength(200)]
    [Display(Name = "Vendor Contact")]
    public string? VendorContact { get; set; }

    [StringLength(200)]
    [Display(Name = "Contact Email")]
    [DataType(DataType.EmailAddress)]
    public string? ContactEmail { get; set; }

    [StringLength(50)]
    [Display(Name = "Contact Phone")]
    [DataType(DataType.PhoneNumber)]
    public string? ContactPhone { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [NotMapped]
    public bool IsExpired => EndDate < DateTime.Today;

    [NotMapped]
    public bool IsExpiringSoon => !IsExpired && EndDate <= DateTime.Today.AddDays(60);

    [NotMapped]
    [Display(Name = "Days Until Expiry")]
    public int DaysUntilExpiry => (EndDate - DateTime.Today).Days;
}
