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
        private readonly CustomerController _customerController;

        public Application(ApplicationDbContext dbContext, DataInitializer dataInitializer, RoomController roomController, CustomerController customerController)
        {
            _dbContext = dbContext;
            _dataInitializer = dataInitializer;
            _roomController = roomController;
            _customerController = customerController;
        }

        public void Run()
        {
            _dataInitializer.MigrateAndSeed(_dbContext);

            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Main Menu")
                        .AddChoices(new[] {
                            "Manage Rooms",
                            "Manage Customers",
                            "Manage Bookings",
                            "Exit"
                        }));

                switch (choice)
                {
                    case "Manage Rooms":
                        _roomController.RoomMenu();
                        break;

                    case "Manage Customers":
                        _customerController.CustomerMenu();
                        break;

                    case "Manage Bookings":
                        AnsiConsole.MarkupLine("[yellow]Booking management is coming soon![/]");
                        Console.ReadKey(true);
                        break;

                    case "Exit":
                        AnsiConsole.MarkupLine("[bold red]Shutting down... Goodbye![/]");
                        return;
                }
            }
        }
    }
}
