using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface ICustomerService
    {
        int AddCustomer(CustomerDTO customerDto);
        List<CustomerDTO> GetAllCustomers();
        CustomerDTO? GetCustomerById(int customerId);
        List<CustomerDTO> SearchCustomer(string name);
        bool UpdateCustomer(CustomerDTO customerDto);
        bool DeleteCustomer(int customerId);

        bool IsEmailUnique(string email);
    }
}
