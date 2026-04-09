using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using HotelManagement.Services;
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

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Customer Management[/]")
                        .AddChoices(new[] {
                            "List All Customers",
                            "Search Customer",
                            "Add New Customer",
                            "Update Customer",
                            "Delete Customer",
                            "Back to Main Menu"
                        }));

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

        private void AddCustomer()
        {
            var customerDto = new CustomerDTO();

            AnsiConsole.MarkupLine("[yellow]Enter customer details:[/]");
            customerDto.FirstName = AnsiConsole.Ask<string>("Enter [blue]First Name[/]:");
            customerDto.LastName = AnsiConsole.Ask<string>("Enter [blue]Last Name[/]:");
            customerDto.Email = AnsiConsole.Ask<string>("Enter [blue]Email[/]:");
            customerDto.Phone = AnsiConsole.Ask<string>("Enter [blue]Phone[/]:");
            customerDto.StreetAddress = AnsiConsole.Ask<string>("Enter [blue]Street Address[/]:");
            customerDto.ZipCode = AnsiConsole.Ask<string>("Enter [blue]Zip Code[/]:");
            customerDto.City = AnsiConsole.Ask<string>("Enter [blue]City[/]:");

            var validationResult = _validator.Validate(customerDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[red]{error.ErrorMessage}[/]");
                }
                return;
            }

            _customerService.AddCustomer(customerDto);
            AnsiConsole.MarkupLine("[green]Customer added successfully![/]");
        }

        private void ListAllCustomers()
        {
            var customers = _customerService.GetAllCustomers();

            if (!customers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers found in system.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Email");
            table.AddColumn("Phone");
            table.AddColumn("Address");
            table.AddColumn("Zipcode");
            table.AddColumn("City");

            foreach (var customer in customers)
            {
                table.AddRow(
                    customer.Id.ToString(),
                    customer.FullName,
                    customer.Email,
                    customer.Phone,
                    customer.StreetAddress,
                    customer.ZipCode,
                    customer.City
                );
            }
            AnsiConsole.Write(table);
        }

        private void SearchCustomer()
        {
            var name = AnsiConsole.Ask<string>("Enter [blue]name[/] to search for:");

            var customerSearch = _customerService.SearchCustomer(name);

            if (!customerSearch.Any())
            {
                AnsiConsole.MarkupLine($"[red]No customers found matching '{name}'.[/]");
                return;
            }

            var table = new Table()
                .Title($"[bold yellow]Search Results for: {name}[/]");
            table.AddColumn("[blue]ID[/]");
            table.AddColumn("[white]Name[/]");
            table.AddColumn("[white]Email[/]");
            table.AddColumn("[white]Phone[/]");
            table.AddColumn("[white]Address[/]");
            table.AddColumn("[white]Zipcode[/]");
            table.AddColumn("[white]City[/]");

            foreach (var customer in customerSearch)
            {
                table.AddRow(
                    customer.Id.ToString(),
                    customer.FullName,
                    customer.Email,
                    customer.Phone,
                    customer.StreetAddress,
                    customer.ZipCode,
                    customer.City
                );
            }
            AnsiConsole.Write(table);
        }

        public void UpdateCustomer()
        {
            var allCustomers = _customerService.GetAllCustomers();

            if (!allCustomers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers available to update.[/]");
                return;
            }

            var selectedCustomer = AnsiConsole.Prompt(
                new SelectionPrompt<CustomerDTO>()
                    .Title("Select a [blue]customer[/] to update:")
                    .PageSize(10)
                    .UseConverter(c => $"{c.FullName} ({c.Email})")
                    .AddChoices(allCustomers));

            var customerToUpdate = _customerService.GetCustomerById(selectedCustomer.Id);

            if (customerToUpdate == null)
            {
                AnsiConsole.MarkupLine("[red]Error: Customer could not be found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Updating: {customerToUpdate.FullName}[/]");

            customerToUpdate.FirstName = AnsiConsole.Ask<string>($"New First Name (Current: [blue]{customerToUpdate.FirstName}[/]):");
            customerToUpdate.LastName = AnsiConsole.Ask<string>($"New Last Name (Current: [blue]{customerToUpdate.LastName}[/]):");
            customerToUpdate.Email = AnsiConsole.Ask<string>($"New Email (Current: [blue]{customerToUpdate.Email}[/]):");
            customerToUpdate.Phone = AnsiConsole.Ask<string>($"New Phone (Current: [blue]{customerToUpdate.Phone}[/]):");
            customerToUpdate.StreetAddress = AnsiConsole.Ask<string>($"New Address (Current: [blue]{customerToUpdate.StreetAddress}[/]):");
            customerToUpdate.ZipCode = AnsiConsole.Ask<string>($"New ZipCode (Current: [blue]{customerToUpdate.ZipCode}[/]):");
            customerToUpdate.City = AnsiConsole.Ask<string>($"New City (Current: [blue]{customerToUpdate.City}[/]):");

            var results = _validator.Validate(customerToUpdate);
            if (!results.IsValid)
            {
                foreach (var error in results.Errors)
                {
                    AnsiConsole.MarkupLine($"[red]{error.ErrorMessage}[/]");
                }
                AnsiConsole.WriteLine("Press any key to return to menu.");
                Console.ReadKey(true);
                return;
            }

            if (_customerService.UpdateCustomer(customerToUpdate))
                AnsiConsole.MarkupLine("\n[green]Customer updated successfully![/]");
            else
                AnsiConsole.MarkupLine("\n[red]Failed to update customer in database.[/]");
        }

        private void DeleteCustomer()
        {
            var id = AnsiConsole.Ask<int>("Enter [red]ID[/] of the customer to delete:");

            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                AnsiConsole.MarkupLine("[red]Customer not found.[/]");
                return;
            }

            if (AnsiConsole.Confirm($"Are you sure you want to delete [yellow]{customer.FullName}[/]?"))
            {
                var success = _customerService.DeleteCustomer(id);
                if (success)
                    AnsiConsole.MarkupLine("[green]Customer deleted.[/]");
                else
                    AnsiConsole.MarkupLine("[red]Could not delete customer. Check if they have active bookings.[/]");
            }
        }

        
    }
}
    

