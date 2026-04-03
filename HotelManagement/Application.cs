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
        private readonly DataInitializer _dataInitializer;

        public Application(ApplicationDbContext context, DataInitializer dataInitializer)
        {
            _context = context;
            _dataInitializer = dataInitializer;
        }

        public void Run()
        {
            _dataInitializer.MigrateAndSeed(_context);
        }
    }
}
