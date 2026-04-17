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
                AnsiConsole.Clear();

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

            customerDto.FirstName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [blue]First Name[/]:")
                    .Validate(s => !string.IsNullOrWhiteSpace(s)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]First name is required.[/]")));

            customerDto.LastName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [blue]Last Name[/]:")
                    .Validate(s => !string.IsNullOrWhiteSpace(s)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Last name is required.[/]")));

            customerDto.Email = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [blue]Email[/]:")
                    .Validate(email => {
                        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                            return ValidationResult.Error("[red]Enter a valid email address[/]");
                        if (!_customerService.IsEmailUnique(email))
                            return ValidationResult.Error("[red]This email is already in use![/]");
                        return ValidationResult.Success();
                    }));

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

            ValidateUI.IsEmpty(customers, "No customers found.");

            TableUI.ShowCustomersTable(customers, "All Customers");
        }

        private void SearchCustomer()
        {
            var name = AnsiConsole.Ask<string>("Enter [blue]Customer Name[/] to search for:");

            var customerSearch = _customerService.SearchCustomer(name);

            ValidateUI.IsEmpty(customerSearch, $"No customers found matching {name}.");

            TableUI.ShowCustomersTable(customerSearch, $"Search Results for: {name}");
        }

        private void UpdateCustomer()
        {
            var allCustomers = _customerService.GetAllCustomers();

            ValidateUI.IsEmpty(allCustomers, "No customers available to update");

            TableUI.ShowCustomersTable(allCustomers, "Select Customer to Update");

            var customerId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [blue]ID[/] of Customer to Update:")
                    .DefaultValue(0)
                    .Validate(id => id == 0 || allCustomers.Any(c => c.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Customer ID.[/]")));

            if (customerId == 0) 
                return;

            var customerToUpdate = allCustomers.First(c => c.Id == customerId);

            AnsiConsole.MarkupLine($"[bold yellow]Updating Customer: {customerToUpdate.FullName}[/]");

            customerToUpdate.FirstName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]First Name[/]:")
                    .DefaultValue(customerToUpdate.FirstName)
                    .Validate(s => !string.IsNullOrWhiteSpace(s)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]First Name is required.[/]")));

            customerToUpdate.LastName = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]Last Name[/]:")
                    .DefaultValue(customerToUpdate.LastName)
                    .Validate(s => !string.IsNullOrWhiteSpace(s)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Last Name is required.[/]")));

            customerToUpdate.Email = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]Email[/]:")
                    .DefaultValue(customerToUpdate.Email)
                    .Validate(email =>
                    {
                        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                            return ValidationResult.Error("[red]Enter a valid email address[/]");

                        var IsEmailUnique = _customerService.IsEmailUnique(email);
                        var IsEmailOwn = email == customerToUpdate.Email;

                        if (!IsEmailOwn && !IsEmailUnique)
                            return ValidationResult.Error("[red]This email is already in use![/]");

                        return ValidationResult.Success();
                    }));

            customerToUpdate.Phone = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]Phone[/]:")
                    .DefaultValue(customerToUpdate.Phone));

            customerToUpdate.StreetAddress = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]Address[/]:")
                    .DefaultValue(customerToUpdate.StreetAddress));

            customerToUpdate.ZipCode = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]ZipCode[/]:")
                    .DefaultValue(customerToUpdate.ZipCode));

            customerToUpdate.City = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter new [blue]City[/]:")
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

            ValidateUI.IsEmpty(customers, "No customers found to delete.");

            TableUI.ShowCustomersTable(customers, "Select Customer to Delete");

            var customerId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter [blue]ID[/] of Customer to Delete:")
                    .DefaultValue(0)
                    .Validate(id => id == 0 || customers.Any(c => c.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid Customer ID.[/]")));

            if (customerId == 0)
                return;

            var customerToDelete = customers.First(c => c.Id == customerId);

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
    

