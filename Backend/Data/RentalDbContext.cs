using Microsoft.EntityFrameworkCore;

public class RentalDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<Apartment> Apartments { get; set; }

    public RentalDbContext(DbContextOptions<RentalDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define relationships
        modelBuilder.Entity<Apartment>()
            .HasOne(a => a.Owner)
            .WithMany(u => u.OwnedApartments)
            .HasForeignKey(a => a.OwnerID);

        modelBuilder.Entity<Apartment>()
            .HasOne(a => a.Renter)
            .WithMany(u => u.RentedApartments)
            .HasForeignKey(a => a.RenterID)
            .IsRequired(false);  // Renter can be null

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Apartment>()
            .HasIndex(a => new { a.FloorID, a.Room })
            .IsUnique();
    }
}