using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.ArrivalDate).IsRequired();
                entity.Property(b => b.DepartureDate).IsRequired();
                entity.Property(b => b.TotalPrice).HasPrecision(9, 2);
                entity.HasQueryFilter(b => !b.IsDeleted);
                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(c => c.Email).IsUnique();
                entity.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.LastName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Phone).HasMaxLength(20);
                entity.Property(c => c.StreetAddress).HasMaxLength(100);
                entity.Property(c => c.ZipCode).HasMaxLength(20);
                entity.Property(c => c.City).HasMaxLength(50);
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.TotalAmount).IsRequired().HasPrecision(9, 2);
                entity.Property(i => i.IssueDate).IsRequired();
                entity.Property(i => i.IsPaid).HasDefaultValue(false);
                entity.HasQueryFilter(i => !i.IsDeleted);
                entity.HasOne(i => i.Booking)
                    .WithOne(b => b.Invoice)
                    .HasForeignKey<Invoice>(i => i.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.RoomNumber).IsRequired();
                entity.HasIndex(r => r.RoomNumber).IsUnique();
                entity.Property(r => r.PricePerNight).IsRequired().HasPrecision(9, 2);
                entity.Property(r => r.Type).IsRequired().HasConversion<string>();
                entity.Property(r => r.Size).IsRequired().HasConversion<string>();
                entity.HasQueryFilter(r => !r.IsDeleted);
            });
        }
    }
}
