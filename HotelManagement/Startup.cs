using Autofac;
using Autofac.Extensions.DependencyInjection;
using HotelManagement.Data;
using HotelManagement.Interfaces;
using HotelManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement
{
    public static class Startup
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.Register(c =>
            {
                var factory = new ApplicationContextFactory();
                return factory.CreateDbContext(Array.Empty<string>());
            }).AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<DataInitializer>().AsSelf();
            builder.RegisterType<Application>().AsSelf();
            builder.RegisterType<RoomService>().As<IRoomService>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
