using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Models;

namespace SoftwareTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<VendorContact> VendorContacts { get; set; }
    public DbSet<SoftwareTitle> SoftwareTitles { get; set; }
    public DbSet<LicensePurchase> LicensePurchases { get; set; }
    public DbSet<MaintenanceContract> MaintenanceContracts { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VendorContact>()
            .HasOne(vc => vc.Vendor)
            .WithMany(v => v.Contacts)
            .HasForeignKey(vc => vc.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SoftwareTitle>()
            .HasOne(st => st.Vendor)
            .WithMany(v => v.SoftwareTitles)
            .HasForeignKey(st => st.VendorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LicensePurchase>()
            .HasOne(lp => lp.SoftwareTitle)
            .WithMany(st => st.LicensePurchases)
            .HasForeignKey(lp => lp.SoftwareTitleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LicensePurchase>()
            .HasOne(lp => lp.Vendor)
            .WithMany(v => v.LicensePurchases)
            .HasForeignKey(lp => lp.VendorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MaintenanceContract>()
            .HasOne(mc => mc.LicensePurchase)
            .WithMany(lp => lp.MaintenanceContracts)
            .HasForeignKey(mc => mc.LicensePurchaseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaintenanceContract>()
            .HasOne(mc => mc.Vendor)
            .WithMany(v => v.MaintenanceContracts)
            .HasForeignKey(mc => mc.VendorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.SoftwareTitle)
            .WithMany(st => st.Subscriptions)
            .HasForeignKey(s => s.SoftwareTitleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Vendor)
            .WithMany(v => v.Subscriptions)
            .HasForeignKey(s => s.VendorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
