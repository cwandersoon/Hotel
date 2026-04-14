using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HotelManagement.DTOs
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int RoomNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public int BookingId { get; set; }
        public string Status => IsPaid ? "Paid" : "Unpaid";
    }
}
