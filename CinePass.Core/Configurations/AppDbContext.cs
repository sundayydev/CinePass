using System;
using CinePass.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CinePass.Core.Configurations
{
    public class AppDbContext : DbContext
    {
        // Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------------
            // Enum conversion (string)
            // --------------------------
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Seat>()
                .Property(s => s.SeatType)
                .HasConversion<string>();

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Booking>()
                .Property(b => b.PaymentMethod)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentMethod)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<string>();

            // --------------------------
            // Constraints / Indexes
            // --------------------------
            // Tránh trùng ghế trong cùng booking
            modelBuilder.Entity<BookingDetail>()
                .HasIndex(bd => new { bd.BookingID, bd.SeatID })
                .IsUnique();

            // Tránh trùng lịch chiếu trong cùng phòng
            modelBuilder.Entity<Showtime>()
                .HasIndex(s => new { s.ScreenID, s.StartTime })
                .IsUnique();

            // --------------------------
            // Cascade delete / relationships
            // --------------------------
            modelBuilder.Entity<Cinema>()
                .HasMany(c => c.Screens)
                .WithOne(s => s.Cinema)
                .HasForeignKey(s => s.CinemaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Screen>()
                .HasMany(s => s.Seats)
                .WithOne(seat => seat.Screen)
                .HasForeignKey(seat => seat.ScreenID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Screen>()
                .HasMany(s => s.Showtimes)
                .WithOne(st => st.Screen)
                .HasForeignKey(st => st.ScreenID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Showtimes)
                .WithOne(st => st.Movie)
                .HasForeignKey(st => st.MovieID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Showtime>()
                .HasMany(st => st.Bookings)
                .WithOne(b => b.Showtime)
                .HasForeignKey(b => b.ShowtimeID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingDetails)
                .WithOne(bd => bd.Booking)
                .HasForeignKey(bd => bd.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seat>()
                .HasMany(s => s.BookingDetails)
                .WithOne(bd => bd.Seat)
                .HasForeignKey(bd => bd.SeatID)
                .OnDelete(DeleteBehavior.Restrict); // tránh xóa ghế ảnh hưởng booking

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingID)
                .OnDelete(DeleteBehavior.Cascade);

            // --------------------------
            // Default values
            // --------------------------
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Movie>()
                .Property(m => m.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Showtime>()
                .Property(st => st.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
