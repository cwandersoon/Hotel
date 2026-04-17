using Bogus;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Data
{
    public class DataInitializer
    {
        private readonly IBookingService _bookingService;
        public DataInitializer(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public void MigrateAndSeed(ApplicationDbContext dbContext)
        {
            dbContext.Database.Migrate();

            SeedCustomers(dbContext);
            SeedRooms(dbContext);
            dbContext.SaveChanges();

            SeedBookings(dbContext);
            dbContext.SaveChanges();

            SeedInvoices(dbContext);
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
                    var roomType = i <= 7 ? RoomType.Single : RoomType.Double;

                    var roomSize = roomType switch
                    {
                        RoomType.Single => RoomSize.Small,
                        RoomType.Double => (i % 2 == 0) ? RoomSize.Medium : RoomSize.Large,
                        _ => RoomSize.Small
                    };

                    var extraBeds = roomSize switch
                    {
                        RoomSize.Small => 0,
                        RoomSize.Medium => 1,
                        RoomSize.Large => 2,
                        _ => 0
                    };

                    rooms.Add(new Room
                    {
                        RoomNumber = 100 + i,
                        Type = roomType,
                        Size = roomSize,
                        PricePerNight = roomType == RoomType.Single ? 1000m : 1500m,
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
                var faker = new Faker("sv");
                var bookings = new List<Booking>();

                for (int i = 0; i < 10; i++)
                {
                    var randomCustomer = faker.PickRandom(customers);
                    var randomRoom = faker.PickRandom(rooms);

                    var createdAt = DateTime.Now.AddDays(-faker.Random.Int(0, 20));
                    var arrival = createdAt.AddDays(faker.Random.Int(1, 14));
                    var departure = arrival.AddDays(faker.Random.Int(1, 7));
                    var extraBeds = faker.Random.Int(0, randomRoom.ExtraBedCapacity);

                    var totalPrice = _bookingService.CalculateTotalPrice(
                        randomRoom.Id,
                        arrival,
                        departure,
                        extraBeds);

                    bookings.Add(new Booking
                    {
                        CreatedAt = createdAt,
                        ArrivalDate = arrival,
                        DepartureDate = departure,
                        CustomerId = randomCustomer.Id,
                        RoomId = randomRoom.Id,
                        ExtraBedsOrdered = faker.Random.Int(0, randomRoom.ExtraBedCapacity),
                        TotalPrice = totalPrice,
                        IsCheckedIn = arrival <= DateTime.Now,
                        IsCheckedOut = departure <= DateTime.Now
                    });
                }

                dbContext.Bookings.AddRange(bookings);
                Console.WriteLine("10 bookings created.");
            }
        }

        private void SeedInvoices(ApplicationDbContext dbContext)
        {
            if (!dbContext.Invoices.Any())
            {
                var bookings = dbContext.Bookings.ToList();
                var faker = new Faker("sv");
                var invoices = new List<Invoice>();

                foreach (var booking in bookings)
                {
                    invoices.Add(new Invoice
                    {
                        BookingId = booking.Id,
                        IssueDate = booking.CreatedAt,
                        DueDate = booking.CreatedAt.AddDays(10),
                        TotalAmount = booking.TotalPrice,
                        IsPaid = faker.Random.Bool(0.1f)
                    });
                }

                dbContext.Invoices.AddRange(invoices);
                Console.WriteLine($"{invoices.Count} invoices generated based on existing bookings.");
            }
        }

    }
}
