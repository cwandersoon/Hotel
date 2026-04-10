using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Controllers
{
    public class BookingController
    {
        private readonly IBookingService _bookingService;
        private readonly ICustomerService _customerService;
        private readonly IRoomService _roomService;
        private readonly IValidator<BookingDTO> _validator;
        private readonly CustomerController _customerController;

        public BookingController(
            IBookingService bookingService,
            ICustomerService customerService,
            IRoomService roomService,
            IValidator<BookingDTO> validator,
            CustomerController customerController)
        {
            _bookingService = bookingService;
            _customerService = customerService;
            _roomService = roomService;
            _validator = validator;
            _customerController = customerController;
        }

        public void BookingMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Booking Management[/]")
                        .AddChoices(new[] {
                            "List All Bookings",
                            "List Bookings by Customer",
                            "Add New Booking",
                            "Update Booking",
                            "Delete Booking",
                            "Back to Main Menu"
                        }));

                switch (choice)
                {
                    case "List All Bookings":
                        ListAllBookings();
                        break;
                    case "List Bookings by Customer":
                        ListBookingsByCustomer();
                        break;
                    case "Add New Booking":
                        AddBooking();
                        break;
                    case "Update Booking":
                        UpdateBooking();
                        break;
                    case "Delete Booking":
                        DeleteBooking();
                        break;
                    case "Back to Main Menu":
                        return;
                }

                AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
                Console.ReadKey(true);
            }
        }

        private void AddBooking()
        {
            CustomerDTO? selectedCustomer = null;

            var customerAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Customer Options:")
                    .AddChoices(new[] { "Select Existing Customer", "Create New Customer", "Cancel" }));

            if (customerAction == "Cancel")
                return;

            if (customerAction == "Create New Customer")
            {
                selectedCustomer = _customerController.AddCustomer();
                if (selectedCustomer == null)
                    return;
            }
            else if (customerAction == "Select Existing Customer")
            {
                var customers = _customerService.GetAllCustomers();

                if (!customers.Any())
                {
                    AnsiConsole.MarkupLine("[red]No customers found. Create a customer first![/]");
                    return;
                }

                selectedCustomer = AnsiConsole.Prompt(
                    new SelectionPrompt<CustomerDTO>()
                        .Title("Select [blue]Customer[/]:")
                        .EnableSearch()
                        .UseConverter(c => $"{c.FullName} ({c.Email})")
                        .AddChoices(customers));
            }

            var arrival = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Arrival date (yyyy-mm-dd):")
                        .Validate(date =>
                        date >= DateTime.Today
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Departure date (yyyy-mm-dd):")
                    .Validate(date =>
                    date > arrival
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure);

            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms available for these dates.[/]");
                return;
            }

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<RoomDTO>()
                    .Title("Select an [blue]Available Room[/]:")
                    .UseConverter(r => $"Room {r.RoomNumber} ({r.Type}) - {r.PricePerNight:C}/night")
                    .AddChoices(availableRooms));

            var extraBeds = AnsiConsole.Ask<int>("Number of [blue]extra beds[/]?");

            var bookingDto = new BookingDTO
            {
                CustomerId = selectedCustomer.Id,
                RoomId = selectedRoom.Id,
                ArrivalDate = arrival,
                DepartureDate = departure,
                ExtraBedsOrdered = extraBeds,
                TotalPrice = _bookingService.CalculateTotalPrice(selectedRoom.Id, arrival, departure, extraBeds)
            };

            var validationResult = _validator.Validate(bookingDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                    AnsiConsole.MarkupLine($"[red]{error.ErrorMessage}[/]");
                return;
            }

            if (AnsiConsole.Confirm($"Total price will be [blue]{bookingDto.TotalPrice:C}[/]. Confirm booking?"))
            {
                _bookingService.AddBooking(bookingDto);
                AnsiConsole.MarkupLine("[green]Booking created successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Booking cancelled.[/]");
            }
        }

        private void ListAllBookings()
        {
            var bookings = _bookingService.GetAllBookings();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No bookings found.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Customer");
            table.AddColumn("Room number");
            table.AddColumn("Dates");
            table.AddColumn("Extra beds");
            table.AddColumn("Total price");

            foreach (var b in bookings)
            {
                table.AddRow(
                    b.Id.ToString(),
                    b.CustomerName,
                    b.RoomNumber.ToString(),
                    $"{b.ArrivalDate:yyyy-MM-dd} to {b.DepartureDate:yyyy-MM-dd}",
                    b.ExtraBedsOrdered.ToString(),
                    $"{b.TotalPrice:C}"
                );
            }
            AnsiConsole.Write(table);
        }

        private void ListBookingsByCustomer()
        {
            var customers = _customerService.GetAllCustomers();

            if (!customers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers found in the system.[/]");
                return;
            }

            var selectedCustomer = AnsiConsole.Prompt(
                new SelectionPrompt<CustomerDTO>()
                    .Title("Select a [blue]customer[/] to see their bookings (type to [blue]search[/]):")
                    .PageSize(15)
                    .UseConverter(c => $"{c.FullName} ({c.Email})")
                    .EnableSearch()
                    .AddChoices(customers));

            var bookings = _bookingService.GetBookingByCustomer(selectedCustomer.Id);

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine($"[red]No bookings found for {selectedCustomer.FullName}.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Customer");
            table.AddColumn("Room number");
            table.AddColumn("Dates");
            table.AddColumn("Extra beds");
            table.AddColumn("Total price");

            foreach (var b in bookings)
            {
                table.AddRow(
                    b.Id.ToString(),
                    b.CustomerName,
                    b.RoomNumber.ToString(),
                    $"{b.ArrivalDate:yyyy-MM-dd} to {b.DepartureDate:yyyy-MM-dd}",
                    b.ExtraBedsOrdered.ToString(),
                    $"{b.TotalPrice:C}"
                );
            }
            AnsiConsole.Write(table);
        }
        private void UpdateBooking()
        {
            var bookings = _bookingService.GetAllBookings();

            if (!bookings.Any())
                return;

            var selectedBooking = AnsiConsole.Prompt(
                new SelectionPrompt<BookingDTO>()
                    .Title("Select [blue]booking to update[/] (type to [blue]search[/]):")
                    .PageSize(10)
                    .EnableSearch()
                    .UseConverter(b => $"#{b.Id}: {b.CustomerName} (Room {b.RoomNumber})")
                    .AddChoices(bookings));

            var arrival = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("New Arrival date:")
                    .DefaultValue(selectedBooking.ArrivalDate)
                    .Validate(date => date >= DateTime.Today ? ValidationResult.Success() : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("New Departure date:")
                    .DefaultValue(selectedBooking.DepartureDate)
                    .Validate(date => date > arrival ? ValidationResult.Success() : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure);

            if (!availableRooms.Any(r => r.Id == selectedBooking.RoomId))
            {
                var currentRoom = _roomService.GetRoomById(selectedBooking.RoomId);
                availableRooms.Add(currentRoom);
            }

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<RoomDTO>()
                    .Title("Select [blue]Room[/]:")
                    .UseConverter(r => $"Room {r.RoomNumber} ({r.Type})")
                    .AddChoices(availableRooms));

            var extraBeds = AnsiConsole.Prompt(
                new TextPrompt<int>("Number of [blue]extra beds[/]?")
                    .DefaultValue(selectedBooking.ExtraBedsOrdered));

            selectedBooking.ArrivalDate = arrival;
            selectedBooking.DepartureDate = departure;
            selectedBooking.RoomId = selectedRoom.Id;
            selectedBooking.ExtraBedsOrdered = extraBeds;
            selectedBooking.TotalPrice = _bookingService.CalculateTotalPrice(selectedRoom.Id, arrival, departure, extraBeds);

            var validationResult = _validator.Validate(selectedBooking);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[red]{error.ErrorMessage}[/]");
                }
                return;
            }

            if (AnsiConsole.Confirm($"New total price: [blue]{selectedBooking.TotalPrice:C}[/]. Confirm update?"))
            {
                if (_bookingService.UpdateBooking(selectedBooking))
                    AnsiConsole.MarkupLine("[green]Booking updated successfully![/]");
                else
                    AnsiConsole.MarkupLine("\n[red]Failed to update bookingt.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Update cancelled.[/]");
            }
        }


        private void DeleteBooking()
        {
            var bookings = _bookingService.GetAllBookings();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No bookings to delete.[/]");
                return;
            }

            var bookingToDelete = AnsiConsole.Prompt(
                new SelectionPrompt<BookingDTO>()
                    .Title("Select a [blue]booking to delete[/] (type to [blue]search[/]):")
                    .PageSize(10)
                    .EnableSearch()
                    .UseConverter(b => $"#{b.Id}: {b.CustomerName} - Room {b.RoomNumber} ({b.ArrivalDate:yyyy-MM-dd})")
                    .AddChoices(bookings));

            AnsiConsole.MarkupLine($"[yellow]You are about to cancel the booking for:[/] [blue]{bookingToDelete.CustomerName}[/]");
            AnsiConsole.MarkupLine($"Room {bookingToDelete.RoomNumber}, Dates: {bookingToDelete.ArrivalDate:yyyy-MM-dd} to {bookingToDelete.DepartureDate:yyyy-MM-dd}");

            if (AnsiConsole.Confirm($"Are you sure you want to delete this booking?"))
            {
                if (_roomService.DeleteRoom(bookingToDelete.Id))
                    AnsiConsole.MarkupLine("\n[green]Booking successfully deleted (soft delete).[/]");
                else
                    AnsiConsole.MarkupLine("\n[red]Could not delete the booking[/]");
            }
        }
    }
}
