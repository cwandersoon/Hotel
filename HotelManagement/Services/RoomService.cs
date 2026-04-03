using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<RoomDTO> GetAllRooms()
        {
            return _context.Rooms
                .Select(r => new RoomDTO
                {
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    ExtraBedCapacity = r.ExtraBedCapacity,
                })
                .ToList();
        }
        public RoomDTO? GetRoomByNumber(int number)
        {
            return _context.Rooms
                .Where(r => r.RoomNumber == number)
                .Select(r => new RoomDTO
                {
                    RoomNumber = r.RoomNumber,
                    Type = r.Type.ToString(),
                    PricePerNight = r.PricePerNight,
                    ExtraBedCapacity = r.ExtraBedCapacity,
                })
                .FirstOrDefault();
        }
        public List<RoomDTO> GetAvailableRooms(DateTime arrival, DateTime departure)
        {
            //wip
            return new List<RoomDTO>();
        }
    }
}
