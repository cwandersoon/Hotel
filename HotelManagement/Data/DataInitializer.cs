using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Data
{
    public class DataInitializer
    {
        public void MigrateAndSeed(ApplicationDbContext context)
        {
            context.Database.Migrate();

            context.SaveChanges();
        }

    }
}
