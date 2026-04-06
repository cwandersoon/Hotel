using Bogus;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Data
{
    public class DataInitializer
    {
        public void MigrateAndSeed(ApplicationDbContext dbContext)
        {
            dbContext.Database.Migrate();

            SeedCustomers(dbContext);
            SeedRooms(dbContext);
            dbContext.SaveChanges();

            SeedBookings(dbContext);
            dbContext.SaveChanges();
        }

        private void SeedCustomers(ApplicationDbContext dbContext)
        {
            if (!dbContext.Customers.Any())
            {
                var customerFaker = new Faker<Customer>("sv")
                    .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                    .RuleFor(c => c.LastName, f => f.Name.LastName())
                    .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
                    .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber("07#-### ## ##"))
                    .RuleFor(c => c.StreetAddress, f => f.Address.StreetAddress())
                    .RuleFor(c => c.ZipCode, f => f.Address.ZipCode())
                    .RuleFor(c => c.City, f => f.Address.City());

                var customers = customerFaker.Generate(25);
                dbContext.Customers.AddRange(customers);
                Console.WriteLine("25 customers generated.");
            }
        }

        private void SeedRooms(ApplicationDbContext dbContext)
        {
            if (!dbContext.Rooms.Any())
            {
                var rooms = new List<Room>();

                for (int i = 1; i <= 15; i++)
                {
                    var roomType = i switch
                    {
                        <= 8 => RoomType.Single,   
                        <= 13 => RoomType.Double,  
                        _ => RoomType.Suite        
                    };

                    var price = roomType switch
                    {
                        RoomType.Single => 795m,
                        RoomType.Double => 1295m,
                        _ => 2495m
                    };

                    var extraBeds = roomType switch
                    {
                        RoomType.Single => 0,
                        RoomType.Double => 1,
                        RoomType.Suite => 2,
                        _ => 0
                    };

                    rooms.Add(new Room
                    {
                        RoomNumber = 100 + i,
                        Type = roomType,
                        PricePerNight = price,
                        ExtraBedCapacity = extraBeds
                    });
                }

                dbContext.Rooms.AddRange(rooms);
                Console.WriteLine("15 rooms created.");
            }
        }

        private void SeedBookings(ApplicationDbContext dbContext)
        {
            if (!dbContext.Bookings.Any())
            {
                var customers = dbContext.Customers.ToList();
                var rooms = dbContext.Rooms.ToList();
                var faker = new Faker();
                var bookings = new List<Booking>();

                for (int i = 0; i < 10; i++)
                {
                    var randomCustomer = faker.PickRandom(customers);
                    var randomRoom = faker.PickRandom(rooms);

                    var arrival = DateTime.Now.AddDays(faker.Random.Int(1, 14));
                    var departure = arrival.AddDays(faker.Random.Int(1, 7));

                    bookings.Add(new Booking
                    {
                        ArrivalDate = arrival,
                        DepartureDate = departure,
                        CustomerId = randomCustomer.Id,
                        RoomId = randomRoom.Id,
                        ExtraBedsOrdered = faker.Random.Int(0, randomRoom.ExtraBedCapacity)
                    });
                }

                dbContext.Bookings.AddRange(bookings);
                Console.WriteLine("10 bookings created.");
            }
        }

    }
}
