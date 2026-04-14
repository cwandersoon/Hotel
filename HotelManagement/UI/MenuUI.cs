using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.UI
{
    public static class MenuUI
    {
        public static string ShowMainMenu()
        {
            AnsiConsole.Clear();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Main Menu[/]")
                    .AddChoices(new[] {
                            "Manage Rooms",
                            "Manage Customers",
                            "Manage Bookings",
                            "Manage Invoices",
                            "[red]Exit[/]"
                    }));
        }
        public static string ShowBookingMenu()
        {
            AnsiConsole.Clear();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Booking Management[/]")
                    .AddChoices(new[] {
                            "List All Bookings",
                            "List Bookings by Customer",
                            "Check In Guest",
                            "Check Out Guest",
                            "Add New Booking",
                            "Update Booking",
                            "Delete Booking",
                            "Back to Main Menu"
                    }));
        }

        public static string ShowCustomerMenu()
        {
            AnsiConsole.Clear();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Customer Management[/]")
                    .AddChoices(new[] {
                            "List All Customers",
                            "Search Customer",
                            "Add New Customer",
                            "Update Customer",
                            "Delete Customer",
                            "Back to Main Menu"
                    }));
        }

        public static string ShowInvoiceMenu()
        {
            AnsiConsole.Clear();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Invoice Management[/]")
                    .AddChoices(new[] {
                            "List All Invoices",
                            "Create Invoice from Booking",
                            "Register Payment",
                            "Update Invoice",
                            "Delete Invoice",
                            "Back to Main Menu"
                    }));
        }

        public static string ShowRoomMenu()
        {
            AnsiConsole.Clear();
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Room Management[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                            "List All Rooms",
                            "Search Available Rooms",
                            "Find Room by Number",
                            "Add New Room",
                            "Update Room",
                            "Delete Room",
                            "Back to Main Menu"
                    }));
        }
    }
}
