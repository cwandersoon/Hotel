using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;


namespace HotelManagement.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CustomerService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int AddCustomer(CustomerDTO customerDto)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChanges();
            return customer.Id;
        }

        public List<CustomerDTO> GetAllCustomers()
        {
            var customers = _dbContext.Customers.ToList();
            return _mapper.Map<List<CustomerDTO>>(customers);
        }

        public CustomerDTO? GetCustomerById(int customerId)
        {
            var customer = _dbContext.Customers.Find(customerId);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public List<CustomerDTO> SearchCustomer(string name)
        {
            var customers = _dbContext.Customers
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name))
                .ToList();
            return _mapper.Map<List<CustomerDTO>>(customers);
        }

        public bool UpdateCustomer(CustomerDTO customerDto)
        {
            var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == customerDto.Id);

            if (customer == null)
                return false;

            _mapper.Map(customerDto, customer);

            _dbContext.SaveChanges();

            return true;
        }
        public bool DeleteCustomer(int customerId)
        {
            var customer = _dbContext.Customers.Find(customerId);

            if (customer == null)
                return false;

            var hasFutureBookings = _dbContext.Bookings
                .Any(b => b.CustomerId == customerId && b.ArrivalDate >= DateTime.Today);

            if (hasFutureBookings)
                return false;

            customer.IsDeleted = true;

            _dbContext.SaveChanges();

            return true;
        }

        public bool IsEmailUnique(string email)
        {
            return !_dbContext.Customers.Any(c => c.Email == email);
        }
    }
}
