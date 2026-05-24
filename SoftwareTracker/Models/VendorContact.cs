using System.ComponentModel.DataAnnotations;

namespace SoftwareTracker.Models;

public class VendorContact
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Vendor")]
    public int VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Contact Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Title / Role")]
    public string? Title { get; set; }

    [StringLength(200)]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [StringLength(50)]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [Display(Name = "Primary Contact")]
    public bool IsPrimary { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
