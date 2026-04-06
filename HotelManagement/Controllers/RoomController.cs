using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Controllers
{
    public class RoomController
    {
        private readonly IRoomService _roomService;
        private readonly IValidator<RoomDTO> _validator;

        public RoomController(IRoomService roomService, IValidator<RoomDTO> validator)
        {
            _roomService = roomService;
            _validator = validator;
        }

        public void RunRoomMenu()
        {
            bool backToMain = false;

            while (!backToMain)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Room Management[/]")
                        .PageSize(10)
                        .AddChoices(new[] {
                            "List All Rooms",
                            "Search Available Rooms",
                            "Find Room by Number",
                            "Add New Room",
                            "Update Room",
                            "Delete Room",
                            "[red]Back to Main Menu[/]"
                        }));


                switch (choice)
                {
                    case "List All Rooms":
                        DisplayAllRooms();
                        break;
                    case "Search Available Rooms":
                        ShowAvailableRooms();
                        break;
                    case "Find Room by Number":
                        FindRoom();
                        break;
                    case "Add New Room":
                        AddNewRoom();
                        break;
                    case "Update Room":
                        UpdateRoom();
                        break;
                    case "Delete Room":
                        DeleteRoom();
                        break;
                    case "[red]Back to Main Menu[/]":
                        backToMain = true;
                        break;
                }
                AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
                Console.ReadKey(true);
            }
        }
        public void AddNewRoom()
        {
            var roomNumber = AnsiConsole.Ask<int>("Enter [blue]room number[/]:");
            var price = AnsiConsole.Ask<decimal>("Enter [blue]price per night[/]:");
            var extraBeds = AnsiConsole.Ask<int>("Enter [blue]extra beds[/]:");

            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select [blue]room type[/]:")
                    .AddChoices(new[] { "Single", "Double", "Suite" }));

            var newRoom = new RoomDTO
            {
                RoomNumber = roomNumber,
                PricePerNight = price,
                Type = type,
                ExtraBedCapacity = extraBeds
            };

            var results = _validator.Validate(newRoom);

            if (!results.IsValid)
            {
                AnsiConsole.MarkupLine("[red]Validation failed:[/]");
                foreach (var error in results.Errors)
                {
                    AnsiConsole.MarkupLine($"- [yellow]{error.ErrorMessage}[/]");
                }
                return;
            }

            _roomService.AddRoom(newRoom);
            AnsiConsole.MarkupLine("\n[bold green]Room added successfully![/]");
        }

        public void DisplayAllRooms()
        {
            var rooms = _roomService.GetAllRooms();

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Room Number");
            table.AddColumn("Type");
            table.AddColumn("Price/Night");
            table.AddColumn("Extra beds");

            foreach (var room in rooms)
            {
                table.AddRow(
                    room.Id.ToString(),
                    room.RoomNumber.ToString(),
                    room.Type,
                    $"{room.PricePerNight:C}",
                    room.ExtraBedCapacity.ToString()
                );
            }

            AnsiConsole.Write(table);
        }

        public void ShowAvailableRooms()
        {
            //Ändra till kalender?
            var arrival = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Arrival date (yyyy-mm-dd):")
                        .Validate(date => date >= DateTime.Today ? ValidationResult.Success() : ValidationResult.Error("[red]Arrival must be today or later[/]")));

            var departure = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Departure date (yyyy-mm-dd):")
                    .Validate(date => date > arrival ? ValidationResult.Success() : ValidationResult.Error("[red]Departure must be after arrival[/]")));

            var availableRooms = _roomService.GetAvailableRooms(arrival, departure);

            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No rooms available for these dates.[/]");
                return;
            }

            var table = new Table()
                .Title($"[green]Available Rooms from {arrival:yyyy-MM-dd} to {departure:yyyy-MM-dd}[/]");
            table.AddColumn("Room Number");
            table.AddColumn("Type");
            table.AddColumn("Price");
            table.AddColumn("Extra beds");

            foreach (var room in availableRooms)
            {
                table.AddRow(
                    room.RoomNumber.ToString(),
                    room.Type,
                    $"{room.PricePerNight:C}",
                    room.ExtraBedCapacity.ToString()
                );
            }

            AnsiConsole.Write(table);
        }

        private void FindRoom()
        {
            var number = AnsiConsole.Ask<int>("Enter [blue]Room Number[/] to search for:");

            var room = _roomService.GetRoomByNumber(number);

            if (room == null)
            {
                AnsiConsole.MarkupLine("[red]Room not found.[/]");
                return;
            }

            var panel = new Panel(
                $"Number: {room.RoomNumber}" +
                $"\nType: {room.Type}" +
                $"\nPrice: {room.PricePerNight:C}" +
                $"\nExtra beds: {room.ExtraBedCapacity}")
            {
                Header = new PanelHeader("Room Details"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
        }

        public void UpdateRoom()
        {
            var allRooms = _roomService.GetAllRooms();

            if (!allRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms available to update.[/]");
                return;
            }

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<RoomDTO>()
                    .Title("Select a [yellow]room to update[/]:")
                    .PageSize(20)
                    .UseConverter(r => $"Room {r.RoomNumber} - {r.Type}")
                    .AddChoices(allRooms));

            var roomToUpdate = _roomService.GetRoomById(selectedRoom.Id);

            if (roomToUpdate == null)
            {
                AnsiConsole.MarkupLine("[red]Error: Room could not be found in database.[/]");
                return;
            }

            var newRoomNumber = AnsiConsole.Ask<int>(
                $"Enter new [blue]number[/] (Current: [blue]{roomToUpdate.RoomNumber}[/])");

            var newPrice = AnsiConsole.Ask<decimal>(
                $"Enter new [blue]price[/] (Current: [blue]{roomToUpdate.PricePerNight:C}[/])");

            var newType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select new [blue]room type[/]:")
                    .AddChoices(new[] { "Single", "Double", "Suite" }));

            var newExtraBeds = AnsiConsole.Ask<int>(
                $"Enter how many [blue]extra beds[/] (Current: [blue]{roomToUpdate.ExtraBedCapacity}[/])");

            roomToUpdate.RoomNumber = newRoomNumber;
            roomToUpdate.PricePerNight = newPrice;
            roomToUpdate.Type = newType;
            roomToUpdate.ExtraBedCapacity = newExtraBeds;

            var results = _validator.Validate(roomToUpdate);
            if (!results.IsValid)
            {
                foreach (var error in results.Errors)
                {
                    AnsiConsole.MarkupLine($"- [yellow]{error.ErrorMessage}[/]");
                }
                AnsiConsole.WriteLine("Press any key to try again.");
                Console.ReadKey(true);
                return;
            }

            if (_roomService.UpdateRoom(roomToUpdate))
                AnsiConsole.MarkupLine("\n[bold green]Room updated successfully![/]");
            else
                AnsiConsole.MarkupLine("\n[bold red]Failed to update room.[/]");
        }

        public void DeleteRoom()
        {
            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No rooms to delete.[/]");
                return;
            }

            var roomToDelete = AnsiConsole.Prompt(
                new SelectionPrompt<RoomDTO>()
                    .Title("Select a [red]room to delete[/]:")
                    .UseConverter(r => $"Room {r.RoomNumber}")
                    .AddChoices(rooms));

            if (AnsiConsole.Confirm($"Are you sure you want to delete room {roomToDelete.RoomNumber}?"))
            {
                if (_roomService.DeleteRoom(roomToDelete.Id))
                    AnsiConsole.MarkupLine("\n[green]Room deleted (soft delete).[/]");
                else
                    AnsiConsole.MarkupLine("\n[red]Cannot delete room with active bookings![/]");
            }
        }

    }
}
