using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface IBookingService
    { 
        int AddBooking(BookingDTO bookingDto);
        List<BookingDTO> GetAllBookings();
        List<BookingDTO> GetBookingByCustomer(int customerId);
        BookingDTO? GetBookingById(int bookingId);
        bool UpdateBooking(BookingDTO bookingDto);
        bool DeleteBooking(int bookingId);


        decimal CalculateTotalPrice(int roomId, DateTime arrival, DateTime departure, int extrabeds);
        bool IsRoomAvailable(int roomId, DateTime arrival, DateTime departure);
        bool CheckExtaBedCapacity(int roomId, int requstedExtraBeds);
        bool ValidateBookingDates(DateTime arrival, DateTime departure);

    }
}
