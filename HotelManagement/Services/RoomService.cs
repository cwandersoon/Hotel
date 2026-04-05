using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _dbContext;

        public RoomService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int AddRoom(RoomDTO roomDto)
        {
            var room = new Room
            {
                RoomNumber = roomDto.RoomNumber,
                PricePerNight = roomDto.PricePerNight,
                ExtraBedCapacity = roomDto.ExtraBedCapacity,
                Type = Enum.Parse<RoomType>(roomDto.Type)
            };

            _dbContext.Rooms.Add(room);
            _dbContext.SaveChanges();

            return room.Id;
        }

        public List<RoomDTO> GetAllRooms()
        {
            return _dbContext.Rooms
                .Select(r => new RoomDTO
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    ExtraBedCapacity = r.ExtraBedCapacity,
                })
                .ToList();
        }

        public RoomDTO? GetRoomByNumber(int number)
        {
            return _dbContext.Rooms
                .Where(r => r.RoomNumber == number)
                .Select(r => new RoomDTO
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    ExtraBedCapacity = r.ExtraBedCapacity,
                })
                .FirstOrDefault();
        }

        public RoomDTO? GetRoomById(int roomId)
        {
            return _dbContext.Rooms
                .Where(r => r.Id == roomId)
                .Select(r => new RoomDTO
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    ExtraBedCapacity = r.ExtraBedCapacity,
                })
                .FirstOrDefault();
        }

        public List<RoomDTO> GetAvailableRooms(DateTime arrival, DateTime departure)
        {
            var occupiedRooms = _dbContext.Bookings
                .Where(b => b.ArrivalDate < departure && b.DepartureDate > arrival)
                .Select(b => b.RoomId)
                .Distinct()
                .ToList();

            return _dbContext.Rooms
                .Where(r => !occupiedRooms.Contains(r.Id))
                .Select(r => new RoomDTO
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight= r.PricePerNight,
                    ExtraBedCapacity = r.ExtraBedCapacity,
                })
                .ToList();
        }

        public bool UpdateRoom(RoomDTO roomDto)
        {
            var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == roomDto.Id);

            if (room == null)
                return false;

            room.RoomNumber = roomDto.RoomNumber;
            room.PricePerNight = roomDto.PricePerNight;
            room.ExtraBedCapacity = roomDto.ExtraBedCapacity;

            if (Enum.TryParse<RoomType>(roomDto.Type, out var roomType))
                room.Type = roomType;

            _dbContext.SaveChanges();

            return true;
        }

        public bool DeleteRoom(int roomId)
        {
            var room = _dbContext.Rooms.Find(roomId);

            if (room == null)
                return false;

            var hasFutureBookings = _dbContext.Bookings
                .Any(b => b.RoomId == roomId && b.ArrivalDate >= DateTime.Today);

            if (hasFutureBookings)
                return false;

            room.IsDeleted = true;

            _dbContext.SaveChanges();

            return true;
        }

        public decimal GetRoomPrice(int roomId)
        {
            var room = _dbContext.Rooms.Find(roomId);

            return room?.PricePerNight ?? 0;
        }

        public bool GetExtraBedCapacity(int roomId, int count)
        {
            var room = _dbContext.Rooms.Find(roomId);

            if (room == null) 
                return false;

            return room.ExtraBedCapacity >= count;
        }

        public bool IsRoomNumberUnique(int roomNumber)
        {
            return !_dbContext.Rooms.Any(r => r.RoomNumber == roomNumber);
        }
    }
}
