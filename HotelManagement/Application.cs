using HotelManagement.Data;
using HotelManagement.Interfaces;
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
        private readonly IRoomService _roomService;

        public Application(ApplicationDbContext context, DataInitializer dataInitializer, IRoomService roomService)
        {
            _context = context;
            _dataInitializer = dataInitializer;
            _roomService = roomService;
        }

        public void Run()
        {
            _dataInitializer.MigrateAndSeed(_context);
        }
    }
}
