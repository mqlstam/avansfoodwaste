using Avans.FoodWaste.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Avans.FoodWaste.Infrastructure.Data
{
    public class FoodWasteDbContext : DbContext
    {
        public FoodWasteDbContext(DbContextOptions<FoodWasteDbContext> options) : base(options)
        {
        }

        public DbSet<Cafeteria> Cafeterias { get; set; }
        public DbSet<CafeteriaStaff> CafeteriaStaff { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CafeteriaStaff>()
                .HasOne(c => c.Cafeteria)
                .WithMany(s => s.Staff)
                .HasForeignKey(s => s.CafeteriaId);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Cafeteria)
                .WithMany(c => c.Packages)
                .HasForeignKey(p => p.CafeteriaId);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.ReservedBy)
                .WithMany()
                .HasForeignKey(p => p.ReservedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Package>()
                .Property(p => p.MealType)
                .HasConversion<int>();
    
            modelBuilder.Entity<Package>()
                .Property(p => p.ReservationStatus)
                .HasConversion<int>();
    
            modelBuilder.Entity<Package>()
                .Property(p => p.NoShowStatus)
                .HasConversion<int>();
            
            modelBuilder.Entity<Package>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); //  Precision 18, Scale 2 is common for currency
            
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Student)
                .WithMany() // A student can have many reservations
                .HasForeignKey(r => r.StudentId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Package)
                .WithMany() // A package can have many reservations (if allowed by your business rules)
                .HasForeignKey(r => r.PackageId);

            base.OnModelCreating(modelBuilder);
        }
    }
}