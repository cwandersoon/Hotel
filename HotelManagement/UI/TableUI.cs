using HotelManagement.DTOs;
using HotelManagement.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.UI
{
    public static class TableUI
    {
        public static void ShowRoomsTable(List<RoomDTO> rooms, string title)
        {
            var table = new Table()
                .Title($"[bold yellow]{title}[/]")
                .Border(TableBorder.Rounded);

            table.AddColumn("ID");
            table.AddColumn("Room Number");
            table.AddColumn("Type");
            table.AddColumn("Size");
            table.AddColumn("Extra beds");
            table.AddColumn("Price/Night");

            foreach (var room in rooms)
            {
                table.AddRow(
                     $"{room.Id}",
                     $"{room.RoomNumber}",
                     $"{room.Type}",
                     $"{room.Size}",
                     $"{room.ExtraBedCapacity}",
                     $"{room.PricePerNight:C}"
                 );
            }
            AnsiConsole.Write(table);
        }
        public static void ShowCustomersTable(List<CustomerDTO> customers, string title)
        {
            var table = new Table()
                .Title($"[bold yellow]{title}[/]")
                .Border(TableBorder.Rounded)
                .Expand();

            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Email");
            table.AddColumn("Phone");
            table.AddColumn("Address");
            table.AddColumn("Zipcode");
            table.AddColumn("City");
            table.AddColumns("Active Bookings");

            foreach (var customer in customers)
            {
                var bookingStatus = customer.HasActiveBookings
                    ? "[green]Yes[/]"
                    : "[red]No[/]";

                table.AddRow(
                    customer.Id.ToString(),
                    customer.FullName,
                    customer.Email,
                    customer.Phone,
                    customer.StreetAddress,
                    customer.ZipCode,
                    customer.City,
                    bookingStatus
                );
            }
            AnsiConsole.Write(table);
        }

        public static void ShowBookingsTable(List<BookingDTO> bookings, string title)
        {
            var table = new Table()
                .Title($"[bold yellow]{title}[/]")
                .Border(TableBorder.Rounded);

            table.AddColumn("ID");
            table.AddColumn("Customer");
            table.AddColumn("Room number");
            table.AddColumn("Room Type");
            table.AddColumn("Dates");
            table.AddColumn("Extra beds");
            table.AddColumn("Total price");

            foreach (var booking in bookings)
            {
                table.AddRow(
                    $"{booking.Id}",
                    $"{booking.CustomerName}",
                    $"{booking.RoomNumber}",
                    $"{booking.RoomType}",
                    $"{booking.ArrivalDate:yyyy-MM-dd} to {booking.DepartureDate:yyyy-MM-dd}",
                    $"{booking.ExtraBedsOrdered}",
                    $"{booking.TotalPrice:C}"
                 );
            }
            AnsiConsole.Write(table);
        }

        public static void ShowInvoicesTable(List<InvoiceDTO> invoices, string title)
        {
            var table = new Table()
                .Title($"[bold yellow]{title}[/]")
                .Border(TableBorder.Rounded);

            table.AddColumn("ID");
            table.AddColumn("Customer");
            table.AddColumn("Room Number");
            table.AddColumn("Total Amount");
            table.AddColumn("Issue Date");
            table.AddColumn("Due Date");
            table.AddColumn("Status");

            foreach (var invoice in invoices)
            {
                var status = invoice.IsPaid
                    ? "[green]Paid[/]"
                    : "[red]Unpaid[/]";

                table.AddRow(
                    $"{invoice.Id}",
                    $"{invoice.CustomerName}",
                    $"{invoice.RoomNumber}",
                    $"{invoice.TotalAmount:C}",
                    $"{invoice.IssueDate:yyyy-MM-dd}",
                    $"{invoice.DueDate:yyyy-MM-dd}",
                    status
                );
            }
            AnsiConsole.Write(table);
        }
    }
}