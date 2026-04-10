using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManagement.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int ExtraBedsOrdered { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public Invoice? Invoice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
