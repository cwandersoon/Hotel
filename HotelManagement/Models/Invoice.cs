using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManagement.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
    }
}
