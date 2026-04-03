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

    }
}
