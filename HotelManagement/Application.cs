using HotelManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement
{
    public class Application
    {
        private readonly ApplicationDbContext _context;

        public Application(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Run()
        {
            var dataInitializer = new DataInitializer();
            dataInitializer.MigrateAndSeed(_context);
        }
    }
}
