using System.ComponentModel.DataAnnotations;

namespace SoftwareTracker.Models;

public class SoftwareTitle
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Software Name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Vendor")]
    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    [Url]
    [Display(Name = "Product Website")]
    public string? Website { get; set; }

    public ICollection<LicensePurchase> LicensePurchases { get; set; } = new List<LicensePurchase>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
