using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManagement.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ExtraBedsOrdered { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
    }
}
