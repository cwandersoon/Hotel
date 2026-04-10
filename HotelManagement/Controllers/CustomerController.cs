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

        public CustomerDTO? AddCustomer()
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
                return null;
            }

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

        private void UpdateCustomer()
        {
            var allCustomers = _customerService.GetAllCustomers();

            if (!allCustomers.Any())
            {
                AnsiConsole.MarkupLine("[red]No customers available to update.[/]");
                return;
            }

            var selectedCustomer = AnsiConsole.Prompt(
                new SelectionPrompt<CustomerDTO>()
                    .Title("Select a [blue]customer[/] to update (type to [blue]search[/]):")
                    .PageSize(10)
                    .EnableSearch()
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

            var selectedCustomer = AnsiConsole.Prompt(
                new SelectionPrompt<CustomerDTO>()
                    .Title("Select a [blue]customer to delete[/] (type to [blue]search[/]):")
                    .PageSize(10)
                    .EnableSearch()
                    .UseConverter(c => $"{c.FullName} ({c.Email})")
                    .AddChoices(customers));

            AnsiConsole.MarkupLine($"[yellow]Warning:[/] You are about to delete [blue]{selectedCustomer.FullName}[/].");

            if (AnsiConsole.Confirm($"Are you sure you want to delete this customer?"))
            {
                if (_customerService.DeleteCustomer(selectedCustomer.Id))
                    AnsiConsole.MarkupLine("[green]Customer deleted successfully.[/]");
                else
                {
                    AnsiConsole.MarkupLine("[red]Could not delete customer.[/]");
                }
            }
        }
    }
}
    

