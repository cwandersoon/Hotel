using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Validators
{
    public class RoomDTOValidator : AbstractValidator<RoomDTO>
    {
        private readonly IRoomService _roomService;
        public RoomDTOValidator(IRoomService roomService)
        {
            _roomService = roomService;

            RuleFor(r => r.RoomNumber)
                .InclusiveBetween(1, 1000)
                .WithMessage("Room number must be between 1 and 1000.");

            RuleFor(r => r.RoomNumber)
                .Must((dto, num) =>
                {
                    if (dto.Id == 0)
                        return _roomService.IsRoomNumberUnique(num);

                    var existingRoom = _roomService.GetRoomById(dto.Id);

                    return _roomService.IsRoomNumberUnique(num) ||
                        (existingRoom != null && existingRoom.RoomNumber == num);
                })
                .WithMessage("Room number already exists.");

            RuleFor(r => r.PricePerNight)
                .InclusiveBetween(100, 999999)
                .WithMessage("Price must be between 100 and 999,999 SEK.");

            RuleFor(r => r.Type)
                .NotEmpty()
                .WithMessage("Room type is required.");

            RuleFor(r => r.ExtraBedCapacity)
                .InclusiveBetween(0, 2)
                .WithMessage("Extra bed capacity must be between 0 and 2.");

        }
    }
}
