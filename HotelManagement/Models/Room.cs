using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManagement.Models
{
    public enum RoomType { Single, Double }
    public enum RoomSize { Small, Medium, Large }

    public class Room
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public RoomType Type { get; set; }
        public RoomSize Size { get; set; }
        public decimal PricePerNight { get; set; }
        public int ExtraBedCapacity { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public bool IsDeleted { get; set; } = false;
    }
}
