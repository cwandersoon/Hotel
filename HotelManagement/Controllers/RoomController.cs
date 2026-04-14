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
    public class RoomController
    {
        private readonly IRoomService _roomService;
        private readonly IValidator<RoomDTO> _validator;

        public RoomController(IRoomService roomService, IValidator<RoomDTO> validator)
        {
            _roomService = roomService;
            _validator = validator;
        }

        public void RoomMenu()
        {
            while (true)
            {
                var choice = MenuUI.ShowRoomMenu();

                switch (choice)
                {
                    case "List All Rooms":
                        DisplayAllRooms();
                        break;
                    case "Search Available Rooms":
                        ShowAvailableRooms();
                        break;
                    case "Find Room by Number":
                        SearchRoom();
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
                    case "Back to Main Menu":
                        return;
                }
                AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
                Console.ReadKey(true);
            }
        }
        public void AddNewRoom()
        {
            var roomDto = new RoomDTO();

            AnsiConsole.MarkupLine("[bold yellow]Enter Room details:[/]");
            roomDto.RoomNumber = AnsiConsole.Ask<int>("Enter [blue]Room Number[/]:");
            roomDto.PricePerNight = AnsiConsole.Ask<decimal>("Enter [blue]Price per night[/]:");
            roomDto.Type = AnsiConsole.Prompt(
                new SelectionPrompt<RoomType>()
                .Title("Select [blue]Room Type[/]:")
                .AddChoices(RoomType.Single, RoomType.Double));

            if (roomDto.Type == RoomType.Double)
            {
                roomDto.Size = AnsiConsole.Prompt(
                    new SelectionPrompt<RoomSize>()
                        .Title("Select [blue]Room Size[/]:")
                        .AddChoices(RoomSize.Medium, RoomSize.Large));

                roomDto.ExtraBedCapacity = (roomDto.Size == RoomSize.Large) ? 2 : 1;
            }
            else if (roomDto.Type == RoomType.Single)
            {
                roomDto.Size = RoomSize.Small;
                roomDto.ExtraBedCapacity = 0;
            }

            if (_validator.IsInvalid(roomDto))
                return;

            _roomService.AddRoom(roomDto);
            AnsiConsole.MarkupLine("\n[green]Room added successfully![/]");
        }

        public void DisplayAllRooms()
        {
            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms found in system.[/]");
                return;
            }
            TableUI.ShowRoomsTable(rooms, "All Rooms");
        }

        public void ShowAvailableRooms()
        {
            AnsiConsole.MarkupLine("[bold yellow]Available Rooms:[/]");
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

            TableUI.ShowRoomsTable(availableRooms, $"Available Rooms from {arrival:yyyy-MM-dd} to {departure:yyyy-MM-dd}");
        }

        private void SearchRoom()
        {
            var number = AnsiConsole.Ask<int>("Enter [blue]Room Number[/] to search for:");

            var room = _roomService.GetRoomByNumber(number);

            if (room == null)
            {
                AnsiConsole.MarkupLine("[red]Room not found.[/]");
                return;
            }

            TableUI.ShowRoomsTable(new List<RoomDTO> { room }, $"Result for Room {number}");
        }

        public void UpdateRoom()
        {
            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms available to update.[/]");
                return;
            }

            TableUI.ShowRoomsTable(rooms, "Select Room to Update");

            var roomId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [blue]ID[/] of the Room to Delete:")
                    .DefaultValue(0));

            if (roomId == 0)
                return;

            var roomToUpdate = rooms.FirstOrDefault(r => r.Id == roomId);

            if (roomToUpdate == null)
            {
                AnsiConsole.MarkupLine("[red]Room could not be found in database.[/]");
                return;
            }

            roomToUpdate.RoomNumber = AnsiConsole.Prompt(
                new TextPrompt<int>($"Enter new [blue]Room Number[/]:")
                    .DefaultValue(roomToUpdate.RoomNumber));

            roomToUpdate.PricePerNight = AnsiConsole.Prompt(
                new TextPrompt<decimal>($"Enter new [blue]Price per night[/]:")
                    .DefaultValue(roomToUpdate.PricePerNight));

            var newType = AnsiConsole.Prompt(
                new SelectionPrompt<RoomType>()
                    .Title("Select new [blue]Room Type[/]:")
                    .AddChoices(RoomType.Single, RoomType.Double));

            if (newType == RoomType.Double)
            {
                roomToUpdate.Size = AnsiConsole.Prompt(
                    new SelectionPrompt<RoomSize>()
                        .Title("Select new [blue]Room Size[/]:")
                        .AddChoices(RoomSize.Medium, RoomSize.Large));

                roomToUpdate.ExtraBedCapacity = (roomToUpdate.Size == RoomSize.Large) ? 2 : 1;
            }
            else if (newType == RoomType.Single)
            {
                roomToUpdate.Size = RoomSize.Small;
                roomToUpdate.ExtraBedCapacity = 0;
            }

            if (_validator.IsInvalid(roomToUpdate))
                return;

            if (_roomService.UpdateRoom(roomToUpdate))
                AnsiConsole.MarkupLine("\n[green]Room updated successfully![/]");
            else
                AnsiConsole.MarkupLine("\n[red]Failed to update room.[/]");
        }

        public void DeleteRoom()
        {
            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]No rooms to delete.[/]");
                return;
            }

            TableUI.ShowRoomsTable(rooms, "Select Room to Delete");

            var roomId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [blue]ID[/] of the Room to Delete:")
                    .DefaultValue(0));

            if (roomId == 0)
                return;

            var roomToDelete = rooms.FirstOrDefault(r => r.Id == roomId);

            if (roomToDelete == null)
            {
                AnsiConsole.MarkupLine("[red]No room found with that ID.[/]");
                return;
            }

            if (AnsiConsole.Confirm($"Are you sure you want to delete room {roomToDelete.RoomNumber}?"))
            {
                if (_roomService.DeleteRoom(roomToDelete.Id))
                    AnsiConsole.MarkupLine("\n[green]Room deleted successfully (soft delete).[/]");
                else
                    AnsiConsole.MarkupLine("\n[red]Cannot delete room with current bookings![/]");
            }
        }

    }
}
