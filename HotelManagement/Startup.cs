using Autofac;
using HotelManagement.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Autofac.Extensions.DependencyInjection;
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

            builder.RegisterType<Application>().AsSelf();

            return builder.Build();
        }
    }
}
