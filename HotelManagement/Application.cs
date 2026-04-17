using HotelManagement.Controllers;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Services;
using HotelManagement.UI;
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
        private readonly BookingController _bookingController;
        private readonly InvoiceController _invoiceController;

        public Application(
            ApplicationDbContext dbContext,
            DataInitializer dataInitializer,
            RoomController roomController,
            CustomerController customerController,
            BookingController bookingController,
            InvoiceController invoiceController)
        {
            _dbContext = dbContext;
            _dataInitializer = dataInitializer;
            _roomController = roomController;
            _customerController = customerController;
            _bookingController = bookingController;
            _invoiceController = invoiceController;
        }

        public void Run()
        {
            _dataInitializer.MigrateAndSeed(_dbContext);

            AnsiConsole.MarkupLine("\n[grey]Press any key to enter Main Menu.[/]");
            Console.ReadKey(true);

            while (true)
            {
                AnsiConsole.Clear();

                var choice = MenuUI.ShowMainMenu();

                switch (choice)
                {
                    case "Manage Rooms":
                        _roomController.RoomMenu();
                        break;
                    case "Manage Customers":
                        _customerController.CustomerMenu();
                        break;
                    case "Manage Bookings":
                        _bookingController.BookingMenu();
                        break;
                    case "Manage Invoices":
                        _invoiceController.InvoiceMenu();
                        break;
                    case "[red]Exit[/]":
                        AnsiConsole.MarkupLine("[red]Exiting![/]");
                        return;
                }
            }
        }
    }
}
