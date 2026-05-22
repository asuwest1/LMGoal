using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Models;

namespace SoftwareTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SoftwareTitle> SoftwareTitles { get; set; }
    public DbSet<LicensePurchase> LicensePurchases { get; set; }
    public DbSet<MaintenanceContract> MaintenanceContracts { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LicensePurchase>()
            .HasOne(lp => lp.SoftwareTitle)
            .WithMany(st => st.LicensePurchases)
            .HasForeignKey(lp => lp.SoftwareTitleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaintenanceContract>()
            .HasOne(mc => mc.LicensePurchase)
            .WithMany(lp => lp.MaintenanceContracts)
            .HasForeignKey(mc => mc.LicensePurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.SoftwareTitle)
            .WithMany(st => st.Subscriptions)
            .HasForeignKey(s => s.SoftwareTitleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
