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

            //CreateMap<Booking, BookingDTO>();
            //CreateMap<BookingDTO, Booking>();

            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();

            //CreateMap<Invoice, InvoiceDTO>();
            //CreateMap<InvoiceDTO, Invoice>();

            CreateMap<Room, RoomDTO>();
            CreateMap<RoomDTO, Room>();

        }
    }
}
