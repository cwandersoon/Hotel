using HotelManagement.Controllers;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement
{
    public class Application
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DataInitializer _dataInitializer;
        private readonly RoomController _roomController;

        public Application(ApplicationDbContext dbContext, DataInitializer dataInitializer, RoomController roomController)
        {
            _dbContext = dbContext;
            _dataInitializer = dataInitializer;
            _roomController = roomController;
        }

        public void Run()
        {
            _dataInitializer.MigrateAndSeed(_dbContext);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Main Menu")
                        .AddChoices(new[] { "Manage Rooms", "Manage Customers", "Exit" }));

                if (choice == "Manage Rooms")
                {
                    _roomController.RunRoomMenu();
                }
                else if (choice == "Exit") break;
            }
        }
    }
}
