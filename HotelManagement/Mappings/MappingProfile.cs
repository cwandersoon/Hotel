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
            CreateMap<Room, RoomDTO>().ReverseMap();

            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.HasActiveBookings, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
                .ForMember(dest => dest.RoomNumber,
                    opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ForMember(dest => dest.RoomType,
                    opt => opt.MapFrom(src => src.Room.Type.ToString()));

            CreateMap<BookingDTO, Booking>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore());

            CreateMap<Invoice, InvoiceDTO>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => $"{src.Booking.Customer.FirstName} {src.Booking.Customer.LastName}"))
                .ForMember(dest => dest.RoomNumber,
                    opt => opt.MapFrom(src => src.Booking.Room.RoomNumber));

            CreateMap<InvoiceDTO, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BookingId, opt => opt.Ignore())
                .ForMember(dest => dest.Booking, opt => opt.Ignore());
        }
    }
}
