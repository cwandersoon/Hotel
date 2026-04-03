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

        [Required]
        [Column(TypeName = "decimal(9,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public bool IsPaid { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
