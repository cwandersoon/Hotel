using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManagement.Models
{
    public enum RoomType { Single, Double, Suite }

    public class Room
    {
        public int Id { get; set; }

        [Required]
        public int RoomNumber { get; set; }

        [Required]
        public RoomType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal PricePerNight { get; set; }

        public int ExtraBedCapacity { get; set; }

        public List<Booking> Bookings { get; set; } = new();
    }
}
