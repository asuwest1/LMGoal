using System.ComponentModel.DataAnnotations;

namespace SoftwareTracker.Models;

public class Vendor
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? Website { get; set; }

    [StringLength(50)]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [StringLength(200)]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public ICollection<VendorContact> Contacts { get; set; } = new List<VendorContact>();
    public ICollection<SoftwareTitle> SoftwareTitles { get; set; } = new List<SoftwareTitle>();
    public ICollection<LicensePurchase> LicensePurchases { get; set; } = new List<LicensePurchase>();
    public ICollection<MaintenanceContract> MaintenanceContracts { get; set; } = new List<MaintenanceContract>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
