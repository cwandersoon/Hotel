using Autofac;
using HotelManagement.Data;
using HotelManagement.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelManagement;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule<HotelModule>();

        var container = builder.Build();

        using var scope = container.BeginLifetimeScope();

        var app = scope.Resolve<Application>();

        app.Run();
    }
}

