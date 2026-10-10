using Microsoft.EntityFrameworkCore;

using TravelAgency.Core.Entities;

namespace TravelAgency.Infrastructure.Data;

public class TravelAgencyDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<TourGroup> TourGroups => Set<TourGroup>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Tourist> Tourists => Set<Tourist>();
    public DbSet<Payment> Payments => Set<Payment>();

    public TravelAgencyDbContext(DbContextOptions<TravelAgencyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштування зв'язків та обмежень за потреби
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Payment)
            .WithOne(p => p.Booking)
            .HasForeignKey<Payment>(p => p.BookingId);
    }
}