using Autofac;
using HotelManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelManagement;
    internal class Program
    {
    static void Main(string[] args)
    {
        var container = Startup.Configure();

        using var scope = container.BeginLifetimeScope();

        var app = scope.Resolve<Application>();

        app.Run();
    }
}

