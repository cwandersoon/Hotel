using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Validators
{
    internal class BookingDTOValidator : AbstractValidator<BookingDTO>
    {
        public BookingDTOValidator(IBookingService bookingService)
        {
            RuleFor(b => b.CustomerId)
                .NotEmpty().WithMessage("A customer must be selected.");

            RuleFor(b => b.RoomId)
                .NotEmpty().WithMessage("A room must be selected.");

            RuleFor(b => b)
                .Must(b => bookingService.ValidateBookingDates(b.ArrivalDate, b.DepartureDate))
                .WithMessage("Invalid booking dates");

            RuleFor(b => b)
                .Must(b => bookingService.IsRoomAvailable(b.RoomId, b.ArrivalDate, b.DepartureDate, b.Id))
                .WithMessage("The selected room is already booked for the chosen period.");

            RuleFor(b => b)
                .Must(b => bookingService.CheckExtaBedCapacity(b.RoomId, b.ExtraBedsOrdered))
                .WithMessage("The selected room does not have enough capacity for the requested extra beds.");

            RuleFor(b => b.ExtraBedsOrdered)
                .InclusiveBetween(0, 2).WithMessage("Invalid number of extra beds.");
        }
    }
}
