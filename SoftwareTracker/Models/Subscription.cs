using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftwareTracker.Models;

public enum BillingPeriod
{
    Monthly,
    Quarterly,
    [Display(Name = "Semi-Annual")]
    SemiAnnual,
    Annual,
    [Display(Name = "Multi-Year")]
    MultiYear
}

public class Subscription
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Software Title")]
    public int SoftwareTitleId { get; set; }
    public SoftwareTitle? SoftwareTitle { get; set; }

    [StringLength(200)]
    [Display(Name = "Subscription ID / Reference")]
    public string? SubscriptionReference { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "End / Renewal Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Cost Per Period")]
    [DataType(DataType.Currency)]
    public decimal? CostPerPeriod { get; set; }

    [Display(Name = "Billing Period")]
    public BillingPeriod BillingPeriod { get; set; } = BillingPeriod.Annual;

    [Display(Name = "Auto-Renews")]
    public bool AutoRenews { get; set; } = true;

    [Range(1, int.MaxValue)]
    [Display(Name = "Seat / User Count")]
    public int? SeatCount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [NotMapped]
    public bool IsExpired => EndDate < DateTime.Today;

    [NotMapped]
    public bool IsExpiringSoon => !IsExpired && EndDate <= DateTime.Today.AddDays(60);

    [NotMapped]
    [Display(Name = "Days Until Renewal")]
    public int DaysUntilRenewal => (EndDate - DateTime.Today).Days;

    [NotMapped]
    [Display(Name = "Annual Cost")]
    public decimal? AnnualCost => BillingPeriod switch
    {
        BillingPeriod.Monthly => CostPerPeriod * 12,
        BillingPeriod.Quarterly => CostPerPeriod * 4,
        BillingPeriod.SemiAnnual => CostPerPeriod * 2,
        BillingPeriod.Annual => CostPerPeriod,
        BillingPeriod.MultiYear => CostPerPeriod,
        _ => CostPerPeriod
    };
}
