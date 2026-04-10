using Autofac;
using AutoMapper;
using FluentValidation;
using HotelManagement.Controllers;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Mappings;
using HotelManagement.Services;
using HotelManagement.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Modules
{
    public class HotelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ApplicationContextFactory()
                .CreateDbContext(Array.Empty<string>()))
                .AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<RoomService>().As<IRoomService>().InstancePerLifetimeScope();
            builder.RegisterType<RoomController>().AsSelf();
            builder.RegisterType<RoomDTOValidator>().As<IValidator<RoomDTO>>().InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerController>().AsSelf();
            builder.RegisterType<CustomerDTOValidator>().As<IValidator<CustomerDTO>>().InstancePerLifetimeScope();
           
            builder.RegisterType<BookingService>().As<IBookingService>().InstancePerLifetimeScope();
            builder.RegisterType<BookingController>().AsSelf();
            builder.RegisterType<BookingDTOValidator>().As<IValidator<BookingDTO>>().InstancePerLifetimeScope();

            builder.RegisterType<DataInitializer>().AsSelf();
            builder.RegisterType<Application>().AsSelf();

            builder.RegisterInstance(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance)).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
               .As<IMapper>().InstancePerLifetimeScope();
        }
    

    }
}
