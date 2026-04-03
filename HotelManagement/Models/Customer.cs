using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HotelManagement.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string StreetAddress { get; set; } = string.Empty;

        [MaxLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        public List<Booking> Bookings { get; set; } = new();
    }
}
