using Bogus.DataSets;
using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using HotelManagement.UI;
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

                var choice = MenuUI.ShowBookingMenu();

                switch (choice)
                {
                    case "List All Bookings":
                        ListAllBookings();
                        break;
                    case "List Bookings by Customer":
                        ListBookingsByCustomer();
                        break;
                    case "Check In Guest":
                        CheckInGuest();
                        break;
                    case "Check Out Guest":
                        CheckOutGuest();
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

            AnsiConsole.MarkupLine("[bold yellow]Enter Booking details:[/]");
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

                ValidateUI.IsEmpty(customers, "No customers found.");

                TableUI.ShowCustomersTable(customers, "Select Customer");

                var customerId = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter Customer [blue]ID[/]:")
                        .Validate(id => id == 0 || customers.Any(c => c.Id == id)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Invalid Customer ID.[/]")));

                if (customerId == 0)
                    return;

                selectedCustomer = customers.First(c => c.Id == customerId);
            }

            var arrival = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Arrival date (yyyy-mm-dd):")
                        .DefaultValue(DateTime.Today)
                        .Validate(date => date >= DateTime.Today
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Departure date (yyyy-mm-dd):")
                    .DefaultValue(DateTime.Today.AddDays(1))
                    .Validate(date => date > arrival
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var numberOfPeople = AnsiConsole.Prompt(
                new TextPrompt<int>("Number of people for the booking?")
                    .Validate(n => n > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Number must be greater than 0.[/]")));

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure, numberOfPeople);

            ValidateUI.IsEmpty(availableRooms, "No rooms available for these dates.");

            TableUI.ShowRoomsTable(availableRooms, $"Available Rooms ({arrival:yyyy-MM-dd} to {departure:yyyy-MM-dd})");

            var roomId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Room [blue]ID[/] to book:")
                    .Validate(id => id == 0 || availableRooms.Any(r => r.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Room ID.[/]")));

            if (roomId == 0)
                return;

            var selectedRoom = availableRooms.First(r => r.Id == roomId);

            int bedsInRoom = selectedRoom.Type == RoomType.Single ? 1 : 2;
            int extraBeds = Math.Max(0, numberOfPeople - bedsInRoom);

            var bookingDto = new BookingDTO
            {
                CustomerId = selectedCustomer!.Id,
                RoomId = selectedRoom.Id,
                ArrivalDate = arrival,
                DepartureDate = departure,
                ExtraBedsOrdered = extraBeds,
                TotalPrice = _bookingService.CalculateTotalPrice(selectedRoom.Id, arrival, departure, extraBeds)
            };

            if (_validator.IsInvalid(bookingDto))
                return;

            if (AnsiConsole.Confirm($"Confirm booking?"))
            {
                _bookingService.AddBooking(bookingDto);
                AnsiConsole.MarkupLine("[green]Booking created successfully![/]");
            }
        }

        private void ListAllBookings()
        {
            var bookings = _bookingService.GetAllBookings();

            ValidateUI.IsEmpty(bookings, "No bookings found.");

            TableUI.ShowBookingsTable(bookings, "All Bookings");
        }

        private void ListBookingsByCustomer()
        {
            var customers = _customerService.GetAllCustomers();

            ValidateUI.IsEmpty(customers, "No customers found.");

            TableUI.ShowCustomersTable(customers, "Select Customer to view Bookings");

            var customerId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Customer [blue]ID[/]:")
                    .Validate(id => id == 0 || customers.Any(c => c.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Customer ID.[/]")));

            if (customerId == 0)
                return;

            var bookings = _bookingService.GetBookingByCustomer(customerId);

            ValidateUI.IsEmpty(bookings, "No bookings found for this customer.");

            TableUI.ShowBookingsTable(bookings, $"Bookings for Customer ID {customerId}");
        }
        private void UpdateBooking()
        {
            var bookings = _bookingService.GetAllBookings();

            ValidateUI.IsEmpty(bookings, "No bookings available to update");

            TableUI.ShowBookingsTable(bookings, "Select Booking to Update");


            var bookingId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Booking [blue]ID[/]:")
                    .Validate(id => id == 0 || bookings.Any(c => c.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Booking ID.[/]")));

            if (bookingId == 0)
                return;

            var selectedBooking = bookings.First(b => b.Id == bookingId);

            var arrival = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter new [blue]Arrival date[/]:")
                    .DefaultValue(selectedBooking.ArrivalDate)
                    .Validate(date => date >= DateTime.Today
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter new [blue]Departure date[/]:")
                    .DefaultValue(selectedBooking.DepartureDate)
                    .Validate(date => date > arrival
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var numberOfPeople = AnsiConsole.Prompt(
               new TextPrompt<int>("Number of people for the booking?")
                   .Validate(n => n > 0
                       ? ValidationResult.Success()
                       : ValidationResult.Error("[red]Number must be greater than 0.[/]")));

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure, numberOfPeople, selectedBooking.Id);

            TableUI.ShowRoomsTable(availableRooms, "Select New Room");

            var roomId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Room [blue]ID[/]:")
                    .Validate(id => id == 0 || availableRooms.Any(r => r.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Room ID.[/]")));

            if (roomId == 0)
                return;

            var selectedRoom = availableRooms.First(r => r.Id == roomId);

            int bedsInRoom = selectedRoom.Type == RoomType.Single ? 1 : 2;
            int extraBeds = Math.Max(0, numberOfPeople - bedsInRoom);

            selectedBooking.ArrivalDate = arrival;
            selectedBooking.DepartureDate = departure;
            selectedBooking.RoomId = selectedRoom.Id;
            selectedBooking.ExtraBedsOrdered = extraBeds;
            selectedBooking.TotalPrice = _bookingService.CalculateTotalPrice(selectedRoom.Id, arrival, departure, extraBeds);
            selectedBooking.IsCheckedIn = false;
            selectedBooking.IsCheckedOut = false;

            if (_validator.IsInvalid(selectedBooking))
                return;

            if (AnsiConsole.Confirm("Confirm update?"))
            {
                if (_bookingService.UpdateBooking(selectedBooking))
                    AnsiConsole.MarkupLine("[green]Booking updated successfully![/]");
                else
                    AnsiConsole.MarkupLine("\n[red]Failed to update bookingt.[/]");
            }
        }

        private void DeleteBooking()
        {
            var bookings = _bookingService.GetAllBookings();

            ValidateUI.IsEmpty(bookings, "No bookings to delete");

            TableUI.ShowBookingsTable(bookings, "Select Booking to Delete");

            var bookingId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Booking [blue]ID[/] to Delete:")
                    .Validate(id => id == 0 || bookings.Any(r => r.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Booking ID.[/]")));

            if (bookingId == 0)
                return;

            var bookingToDelete = bookings.First(b => b.Id == bookingId);


            AnsiConsole.MarkupLine($"You are about to cancel the booking for: [blue]{bookingToDelete.CustomerName}[/]");

            if (AnsiConsole.Confirm("Are you sure you want to delete this booking?"))
            {
                if (_bookingService.DeleteBooking(bookingToDelete.Id))
                    AnsiConsole.MarkupLine("\n[green]Booking deleted successfully (soft delete).[/]");
                else
                    AnsiConsole.MarkupLine("\n[red]Could not delete the booking[/]");
            }
        }
        private void CheckInGuest()
        {
            var bookings = _bookingService.GetAllBookings()
                .Where(b => !b.IsCheckedIn && !b.IsCheckedOut && b.ArrivalDate.Date <= DateTime.Today)
                .ToList();

            ValidateUI.IsEmpty(bookings, "No guest available to check in.");

            TableUI.ShowBookingsTable(bookings, "Available for Check-In");

            var bookingId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Booking [blue]ID[/] to Check In:")
                    .Validate(id => id == 0 || bookings.Any(r => r.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Booking ID.[/]")));

            if (bookingId == 0)
                return;

            if (_bookingService.CheckIn(bookingId))
                AnsiConsole.MarkupLine("[green]Guest is checked in![/]");
        }
        private void CheckOutGuest()
        {
            var bookings = _bookingService.GetAllBookings()
                .Where(b => b.IsCheckedIn && !b.IsCheckedOut)
                .ToList();

            ValidateUI.IsEmpty(bookings, "No guests are currently checked in.");

            TableUI.ShowBookingsTable(bookings, "Available for Check-Out");

            var bookingId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Booking [blue]ID[/] to Check Out:")
                    .Validate(id => id == 0 || bookings.Any(r => r.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Booking ID.[/]")));

            if (bookingId == 0)
                return;

            if (_bookingService.CheckOut(bookingId))
                AnsiConsole.MarkupLine("[green]Guest is checked out![/]");
            else
                AnsiConsole.MarkupLine("[red]You cannot check out a guest that is not checked in yet![/]");
        }
    }
}
