using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement
{
    public class Application
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DataInitializer _dataInitializer;
        private readonly IRoomService _roomService;

        public Application(ApplicationDbContext dbContext, DataInitializer dataInitializer, IRoomService roomService)
        {
            _dbContext = dbContext;
            _dataInitializer = dataInitializer;
            _roomService = roomService;
        }

        public void Run()
        {
            _dataInitializer.MigrateAndSeed(_dbContext);

        }
    }
}
