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

                if (!customers.Any())
                {
                    AnsiConsole.MarkupLine("[red]No customers found. Create a customer first![/]");
                    return;
                }

                TableUI.ShowCustomersTable(customers, "Select Customer");

                var customerId = AnsiConsole.Ask<int>("Enter Customer [blue]ID[/]");

                if (customerId == 0)
                    return;

                selectedCustomer = customers.FirstOrDefault(c => c.Id == customerId);

                if (selectedCustomer == null)
                {
                    AnsiConsole.MarkupLine("[red]Customer not found.[/]");
                    return;
                }
            }

            var arrival = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Arrival date (yyyy-mm-dd):")
                        .DefaultValue(DateTime.Today)
                        .Validate(date =>
                        date >= DateTime.Today
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Departure date (yyyy-mm-dd):")
                    .DefaultValue(DateTime.Today.AddDays(1))
                    .Validate(date =>
                    date > arrival
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var numberOfPeople = AnsiConsole.Ask<int>("Number of people for the booking?");

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure, numberOfPeople);

            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms available for these dates.[/]");
                return;
            }

            TableUI.ShowRoomsTable(availableRooms, $"Available Rooms ({arrival:yyyy-MM-dd} to {departure:yyyy-MM-dd})");

            var roomId = AnsiConsole.Ask<int>("Enter Room [blue]ID[/] to book:");

            if (roomId == 0)
                return;

            var selectedRoom = availableRooms.FirstOrDefault(r => r.Id == roomId);

            if (selectedRoom == null)
            {
                AnsiConsole.MarkupLine("[red]Invalid Room ID.[/]");
                return;
            }

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

            TableUI.ShowBookingsTable(bookings, "All Bookings");
        }

        private void ListBookingsByCustomer()
        {
            var customers = _customerService.GetAllCustomers();

            if (!customers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers found in the system.[/]");
                return;
            }

            TableUI.ShowCustomersTable(customers, "Select Customer to view Bookings")
                ;
            var customerId = AnsiConsole.Ask<int>("Enter Customer [blue]ID[/]:");

            var bookings = _bookingService.GetBookingByCustomer(customerId);

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine($"[red]No bookings found for this customer.[/]");
                return;
            }

            TableUI.ShowBookingsTable(bookings, $"Bookings for Customer ID {customerId}");
        }
        private void UpdateBooking()
        {
            var bookings = _bookingService.GetAllBookings();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No bookings available to update.[/]");
                return;
            }

            TableUI.ShowBookingsTable(bookings, "Select Booking to Update");

            var bookingId = AnsiConsole.Ask<int>("Enter Booking [blue]ID[/] to Update:");

            var selectedBooking = bookings.FirstOrDefault(b => b.Id == bookingId);

            if (selectedBooking == null)
                return;

            var arrival = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter new [blue]Arrival date[/]:")
                    .DefaultValue(selectedBooking.ArrivalDate)
                    .Validate(date => date >= DateTime.Today ? ValidationResult.Success() : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter new [blue]Departure date[/]:")
                    .DefaultValue(selectedBooking.DepartureDate)
                    .Validate(date => date > arrival ? ValidationResult.Success() : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var numberOfPeople = AnsiConsole.Ask<int>("Number of people for the booking?");

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure, numberOfPeople, selectedBooking.Id);

            TableUI.ShowRoomsTable(availableRooms, "Select New Room");

            var roomId = AnsiConsole.Ask<int>("Enter Room [blue]ID[/]:");

            var selectedRoom = availableRooms.FirstOrDefault(r => r.Id == roomId);

            if (selectedRoom == null)
                return;

            int bedsInRoom = selectedRoom.Type == RoomType.Single ? 1 : 2;
            int extraBeds = Math.Max(0, numberOfPeople - bedsInRoom);

            selectedBooking.ArrivalDate = arrival;
            selectedBooking.DepartureDate = departure;
            selectedBooking.RoomId = selectedRoom.Id;
            selectedBooking.ExtraBedsOrdered = extraBeds;
            selectedBooking.TotalPrice = _bookingService.CalculateTotalPrice(selectedRoom.Id, arrival, departure, extraBeds);

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

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No bookings to delete.[/]");
                return;
            }

            TableUI.ShowBookingsTable(bookings, "Select Booking to Delete");

            var bookingId = AnsiConsole.Ask<int>("Enter Booking [blue]ID[/] to Delete:");

            var bookingToDelete = bookings.FirstOrDefault(b => b.Id == bookingId);

            if (bookingToDelete == null)
                return;

            AnsiConsole.MarkupLine($"You are about to cancel the booking for: [blue]{bookingToDelete.CustomerName}[/]");
            AnsiConsole.MarkupLine($"Room {bookingToDelete.RoomNumber}, Dates: {bookingToDelete.ArrivalDate:yyyy-MM-dd} to {bookingToDelete.DepartureDate:yyyy-MM-dd}");

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
                .Where(b => !b.IsCheckedIn && b.ArrivalDate.Date <= DateTime.Today)
                .ToList();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No guests to check in.[/]");
                return;
            }

            TableUI.ShowBookingsTable(bookings, "Check-In");

            var bookingId = AnsiConsole.Ask<int>("Enter Booking [blue]ID[/] to Check In:");

            if (_bookingService.CheckIn(bookingId))
                AnsiConsole.MarkupLine("[green]Guest is checked in![/]");
        }
        private void CheckOutGuest()
        {
            var bookings = _bookingService.GetAllBookings().Where(b => !b.IsCheckedOut).ToList();

            TableUI.ShowBookingsTable(bookings, "Check-Out");

            var bookingId = AnsiConsole.Ask<int>("Enter Booking [blue]ID[/] to Check Out:");

            if (_bookingService.CheckOut(bookingId))
                AnsiConsole.MarkupLine("[green]Guest is checked out![/]");
            else
                AnsiConsole.MarkupLine("[red]You cannot check out a guest that is not checked in yet![/]");
        }
    }
}
