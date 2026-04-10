using AutoMapper;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HotelManagement.Services
{
    internal class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookingService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int AddBooking(BookingDTO bookingDto)
        {
            var booking = _mapper.Map<Booking>(bookingDto);
            _dbContext.Bookings.Add(booking);
            _dbContext.SaveChanges();
            return booking.Id;
        }

        public List<BookingDTO> GetAllBookings()
        {
            var bookings = _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .ToList();
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        public List<BookingDTO> GetBookingByCustomer(int customerId)
        {
            var bookings = _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.CustomerId == customerId)
                .ToList();
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        public BookingDTO? GetBookingById(int bookingId)
        {
            var booking = _dbContext.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == bookingId);
            return _mapper.Map<BookingDTO>(booking);
        }

        public bool UpdateBooking(BookingDTO bookingDto)
        {
            var booking = _dbContext.Bookings.FirstOrDefault(b => b.Id == bookingDto.Id);

            if (booking == null)
                return false;

            _mapper.Map(bookingDto, booking);

            _dbContext.SaveChanges();

            return true;
        }

        public bool DeleteBooking(int bookingId)
        {
            var booking = _dbContext.Bookings.Find(bookingId);

            if (booking == null)
                return false;

            booking.IsDeleted = true;

            _dbContext.SaveChanges();

            return true;
        }


        public decimal CalculateTotalPrice(int roomId, DateTime arrival, DateTime departure, int extrabeds)
        {
            var room = _dbContext.Rooms.Find(roomId);

            if (room == null)
                return 0;

            int totalNights = (int)(departure.Date - arrival.Date).TotalDays;

            decimal extraBedCostPerNight = 200m;

            decimal roomCost = room.PricePerNight * totalNights;
            decimal extraBedsCost = (extrabeds * extraBedCostPerNight) * totalNights;

            return roomCost + extraBedsCost;
        }

        public bool CheckExtaBedCapacity(int roomId, int requstedExtraBeds)
        {
            if (requstedExtraBeds == 0) return true;

            var room = _dbContext.Rooms.Find(roomId);
            if (room == null) return false;

            return requstedExtraBeds <= room.ExtraBedCapacity;
        }

        public bool IsRoomAvailable(int roomId, DateTime arrival, DateTime departure, int? currentBookingId = null)
        {
            var query = _dbContext.Bookings.AsQueryable();

            if (currentBookingId.HasValue)
            {
                query = query.Where(b => b.Id != currentBookingId.Value);
            }

            bool isOccupied = query.Any(b => b.RoomId == roomId &&
                                             b.ArrivalDate < departure.Date &&
                                             b.DepartureDate > arrival.Date);

            return !isOccupied;
        }

        public bool ValidateBookingDates(DateTime arrival, DateTime departure)
        {
            if (arrival >= departure) return false;
            if (arrival.Date < DateTime.Now.Date) return false;

            return true;
        }
    }
}
