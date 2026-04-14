using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using HotelManagement.Services;
using HotelManagement.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Controllers
{
    public class CustomerController
    {
        private readonly ICustomerService _customerService;
        private readonly IValidator<CustomerDTO> _validator;

        public CustomerController(ICustomerService customerService, IValidator<CustomerDTO> validator)
        {
            _customerService = customerService;
            _validator = validator;
        }

        public void CustomerMenu()
        {
            while (true)
            {
                var choice = MenuUI.ShowCustomerMenu();

                switch (choice)
                {
                    case "List All Customers":
                        ListAllCustomers();
                        break;
                    case "Search Customer":
                        SearchCustomer();
                        break;
                    case "Add New Customer":
                        AddCustomer();
                        break;
                    case "Update Customer":
                        UpdateCustomer();
                        break;
                    case "Delete Customer":
                        DeleteCustomer();
                        break;
                    case "Back to Main Menu":
                        return;
                }

                AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
                Console.ReadKey(true);
            }
        }

        public CustomerDTO? AddCustomer()
        {
            var customerDto = new CustomerDTO();

            AnsiConsole.MarkupLine("[bold yellow]Enter customer details:[/]");
            customerDto.FirstName = AnsiConsole.Ask<string>("Enter [blue]First Name[/]:");
            customerDto.LastName = AnsiConsole.Ask<string>("Enter [blue]Last Name[/]:");
            customerDto.Email = AnsiConsole.Ask<string>("Enter [blue]Email[/]:");
            customerDto.Phone = AnsiConsole.Ask<string>("Enter [blue]Phone[/]:");
            customerDto.StreetAddress = AnsiConsole.Ask<string>("Enter [blue]Street Address[/]:");
            customerDto.ZipCode = AnsiConsole.Ask<string>("Enter [blue]Zip Code[/]:");
            customerDto.City = AnsiConsole.Ask<string>("Enter [blue]City[/]:");

            if (_validator.IsInvalid(customerDto))
                return null;

            int newId = _customerService.AddCustomer(customerDto);
            customerDto.Id = newId;

            AnsiConsole.MarkupLine("[green]Customer added successfully![/]");
            return customerDto;
        }

        private void ListAllCustomers()
        {
            var customers = _customerService.GetAllCustomers();

            if (!customers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers found in system.[/]");
                return;
            }

            TableUI.ShowCustomersTable(customers, "All Customers");
        }

        private void SearchCustomer()
        {
            var name = AnsiConsole.Ask<string>("Enter [blue]Customer Name[/] to search for:");

            var customerSearch = _customerService.SearchCustomer(name);

            if (!customerSearch.Any())
            {
                AnsiConsole.MarkupLine($"[red]No customers found matching '{name}'.[/]");
                return;
            }

            TableUI.ShowCustomersTable(customerSearch, $"Search Results for: {name}");
        }

        private void UpdateCustomer()
        {
            var allCustomers = _customerService.GetAllCustomers();

            if (!allCustomers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers available to update.[/]");
                return;
            }

            TableUI.ShowCustomersTable(allCustomers, "Select Customer to Update");
            var customerId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [blue]ID[/] of the Customer to Update:")
                    .DefaultValue(0));

            if (customerId == 0) 
                return;

            var customerToUpdate = allCustomers.FirstOrDefault(c => c.Id == customerId);

            if (customerToUpdate == null)
            {
                AnsiConsole.MarkupLine("[red]Customer could not be found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[bold yellow]Updating Customer: {customerToUpdate.FullName}[/]");

            customerToUpdate.FirstName = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]First Name[/]:")
                    .DefaultValue(customerToUpdate.FirstName));

            customerToUpdate.LastName = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]Last Name[/]:")
                    .DefaultValue(customerToUpdate.LastName));

            customerToUpdate.Email = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]Email[/]:")
                    .DefaultValue(customerToUpdate.Email));

            customerToUpdate.Phone = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]Phone[/]:")
                    .DefaultValue(customerToUpdate.Phone));

            customerToUpdate.StreetAddress = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]Address[/]:")
                    .DefaultValue(customerToUpdate.StreetAddress));

            customerToUpdate.ZipCode = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]ZipCode[/]:")
                    .DefaultValue(customerToUpdate.ZipCode));

            customerToUpdate.City = AnsiConsole.Prompt(
                new TextPrompt<string>("New [blue]City[/]:")
                    .DefaultValue(customerToUpdate.City));

            if (_validator.IsInvalid(customerToUpdate))
                return;

            if (_customerService.UpdateCustomer(customerToUpdate))
                AnsiConsole.MarkupLine("\n[green]Customer updated successfully![/]");
            else
                AnsiConsole.MarkupLine("\n[red]Failed to update customer.[/]");
        }

        private void DeleteCustomer()
        {
            var customers = _customerService.GetAllCustomers();

            if (!customers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers found to delete.[/]");
                return;
            }

            TableUI.ShowCustomersTable(customers, "Select Customer to Delete");

            var customerId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [blue]ID[/] of the Customer to Delete:")
                    .DefaultValue(0));

            if (customerId == 0)
                return;

            var customerToDelete = customers.FirstOrDefault(c => c.Id == customerId);

            if (customerToDelete == null)
            {
                AnsiConsole.MarkupLine("[red]Customer not found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"You are about to delete customer: [blue]{customerToDelete.FullName}[/].");

            if (AnsiConsole.Confirm($"Are you sure you want to delete this customer?"))
            {
                if (_customerService.DeleteCustomer(customerToDelete.Id))
                    AnsiConsole.MarkupLine("[green]Customer deleted successfully (soft delete).[/]");
                else
                {
                    AnsiConsole.MarkupLine("[red]Cannot delete customer with current bookings.[/]");
                }
            }
        }
    }
}
    

