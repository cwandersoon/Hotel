using AutoMapper;
using HotelManagement.DTOs;
using HotelManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Room, RoomDTO>().ReverseMap();

            CreateMap<Booking, BookingDTO>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
            .ForMember(dest => dest.RoomNumber,
                opt => opt.MapFrom(src => src.Room.RoomNumber));

            CreateMap<BookingDTO, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Room, opt => opt.Ignore());
        }
    }
}
