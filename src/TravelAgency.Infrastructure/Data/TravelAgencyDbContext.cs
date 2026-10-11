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
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Setting> Settings => Set<Setting>();

    public TravelAgencyDbContext(DbContextOptions<TravelAgencyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(255).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(100).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Tour>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(100).IsRequired();
            e.Property(x => x.Country).HasMaxLength(50).IsRequired();
            e.Property(x => x.Cities).HasMaxLength(200).IsRequired();
            e.Property(x => x.TransportType).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.MealPlan).HasConversion<string>().HasMaxLength(10);
            e.Property(x => x.AccommodationType).HasMaxLength(50).IsRequired();
            e.Property(x => x.BasePrice).HasPrecision(10, 2);
            e.HasIndex(x => x.Country);
            e.HasIndex(x => x.BasePrice);
            e.ToTable(t => t.HasCheckConstraint("CK_Tours_BasePrice", "[BasePrice] >= 0"));
        });

        modelBuilder.Entity<Hotel>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Country).HasMaxLength(50).IsRequired();
            e.Property(x => x.City).HasMaxLength(50).IsRequired();
            e.Property(x => x.Address).HasMaxLength(150).IsRequired();
            e.Property(x => x.HotelType).HasMaxLength(30).IsRequired();
            e.ToTable(t => t.HasCheckConstraint("CK_Hotels_Capacity", "[Capacity] > 0"));
        });

        modelBuilder.Entity<TourGroup>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.DepartureDate);
            e.HasOne(x => x.Tour).WithMany(t => t.TourGroups)
                .HasForeignKey(x => x.TourId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Hotel).WithMany()
                .HasForeignKey(x => x.HotelId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_TourGroups_Seats", "[SeatsAvailable] >= 0 AND [SeatsAvailable] <= [SeatsTotal]");
                t.HasCheckConstraint("CK_TourGroups_Dates", "[ArrivalDate] >= [DepartureDate]");
            });
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.Property(x => x.TotalPrice).HasPrecision(12, 2);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Status);
            e.HasOne(x => x.User).WithMany(u => u.Bookings)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.TourGroup).WithMany(g => g.Bookings)
                .HasForeignKey(x => x.TourGroupId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<User>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => t.HasCheckConstraint("CK_Bookings_Persons", "[PersonsCount] >= 1"));
        });

        modelBuilder.Entity<Tourist>(e =>
        {
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.PassportEncrypted).IsRequired();
            e.Property(x => x.Gender).HasConversion<string>().HasMaxLength(10);
            e.Property(x => x.ChildrenInfo).HasMaxLength(50);
            e.Property(x => x.TicketPrice).HasPrecision(10, 2);
            e.HasOne(x => x.Booking).WithMany(b => b.Tourists)
                .HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Cascade);
            e.ToTable(t => t.HasCheckConstraint("CK_Tourists_Age", "[Age] > 0"));
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.Property(x => x.Amount).HasPrecision(12, 2);
            e.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Provider).HasMaxLength(50).IsRequired();
            e.Property(x => x.ExternalTransactionId).HasMaxLength(100);
            e.HasOne(x => x.Booking).WithMany(b => b.Payments)
                .HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.Channel).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Message).IsRequired();
            e.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Booking).WithMany()
                .HasForeignKey(x => x.BookingId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Setting>(e =>
        {
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasMaxLength(100);
            e.Property(x => x.Value).HasMaxLength(500).IsRequired();
        });
    }
}
