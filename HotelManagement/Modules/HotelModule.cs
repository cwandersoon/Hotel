using Autofac;
using AutoMapper;
using HotelManagement.Data;
using HotelManagement.Interfaces;
using HotelManagement.Mappings;
using HotelManagement.Services;
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

            builder.RegisterType<DataInitializer>().AsSelf();
            builder.RegisterType<RoomService>().As<IRoomService>().InstancePerLifetimeScope();
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
