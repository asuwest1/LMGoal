namespace SoftwareTracker.Models;

public class DashboardViewModel
{
    public int TotalSoftwareTitles { get; set; }
    public int TotalLicensePurchases { get; set; }
    public int TotalMaintenanceContracts { get; set; }
    public int TotalSubscriptions { get; set; }

    public int ActiveMaintenanceContracts { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiredMaintenanceContracts { get; set; }
    public int ExpiredSubscriptions { get; set; }

    public List<MaintenanceContract> ExpiringSoonContracts { get; set; } = new();
    public List<Subscription> ExpiringSoonSubscriptions { get; set; } = new();
    public List<MaintenanceContract> RecentlyExpiredContracts { get; set; } = new();
    public List<Subscription> RecentlyExpiredSubscriptions { get; set; } = new();

    public decimal? TotalAnnualSubscriptionCost { get; set; }
    public decimal? TotalAnnualMaintenanceCost { get; set; }
}
