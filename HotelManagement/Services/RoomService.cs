using AutoMapper;
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
        private readonly IMapper _mapper;

        public RoomService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int AddRoom(RoomDTO roomDto)
        {
            var room = _mapper.Map<Room>(roomDto);

            _dbContext.Rooms.Add(room);
            _dbContext.SaveChanges();

            return room.Id;
        }

        public List<RoomDTO> GetAllRooms()
        {
            var rooms = _dbContext.Rooms.ToList();
            return _mapper.Map<List<RoomDTO>>(rooms);
        }

        public RoomDTO GetRoomByNumber(int number)
        {
            var room = _dbContext.Rooms.FirstOrDefault(r => r.RoomNumber == number);
            return _mapper.Map<RoomDTO>(room);
        }

        public RoomDTO GetRoomById(int roomId)
        {
            var room = _dbContext.Rooms.Find(roomId);
            return _mapper.Map<RoomDTO>(room);
        }

        public List<RoomDTO> GetAvailableRooms(DateTime arrival, DateTime departure, int numberOfPeople, int? currentBookingId = null)
        {
            var occupiedRooms = _dbContext.Bookings
                .Where(b => b.ArrivalDate < departure && b.DepartureDate > arrival && b.Id != currentBookingId)
                .Select(b => b.RoomId)
                .Distinct()
                .ToList();

            var availableRooms = _dbContext.Rooms
                .Where(r => !occupiedRooms.Contains(r.Id))
                .AsEnumerable()
                .Where(r => {
                    int beds = (r.Type == RoomType.Single) ? 1 : 2;
                    return (beds + r.ExtraBedCapacity) >= numberOfPeople;
                })
                .ToList();

            return _mapper.Map<List<RoomDTO>>(availableRooms);
        }

        public bool UpdateRoom(RoomDTO roomDto)
        {
            var room = _dbContext.Rooms.FirstOrDefault(r => r.Id == roomDto.Id);

            if (room == null)
                return false;

            _mapper.Map(roomDto, room);

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

        public bool IsRoomNumberUnique(int roomNumber)
        {
            return !_dbContext.Rooms.Any(r => r.RoomNumber == roomNumber);
        }
    }
}
