using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManagement.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int ExtraBedCapacity { get; set; }
    }
}
