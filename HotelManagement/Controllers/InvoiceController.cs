using Bogus.DataSets;
using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using HotelManagement.UI;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HotelManagement.Controllers
{
    public class InvoiceController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IBookingService _bookingService;
        private readonly IValidator<InvoiceDTO> _validator;

        public InvoiceController(
            IInvoiceService invoiceService,
            IBookingService bookingService,
            IValidator<InvoiceDTO> validator)
        {
            _invoiceService = invoiceService;
            _bookingService = bookingService;
            _validator = validator;
        }

        public void InvoiceMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();

                int expiredCount = _bookingService.DeleteExpiredBookings();

                if (expiredCount > 0)
                    AnsiConsole.MarkupLine($"[red]{expiredCount} Bookings with unpaid Invoices are cancelled.[/]");

                var choice = MenuUI.ShowInvoiceMenu();

                switch (choice)
                {
                    case "List All Invoices":
                        ListAllInvoices();
                        break;
                    case "Create Invoice from Booking":
                        AddInvoice();
                        break;
                    case "Register Payment":
                        RegisterPayment();
                        break;
                    case "Update Invoice":
                        UpdateInvoice();
                        break;
                    case "Delete Invoice":
                        DeleteInvoice();
                        break;
                    case "Back to Main Menu":
                        return;
                }

                AnsiConsole.MarkupLine("\n[grey]Press any key to return to menu...[/]");
                Console.ReadKey(true);
            }
        }

        private void AddInvoice()
        {
            var bookings = _bookingService.GetBookingsWithoutInvoice();

            ValidateUI.IsEmpty(bookings, "No bookings available to invoice.");

            TableUI.ShowBookingsTable(bookings, "Bookings available for Invoice");

            var bookingId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Booking [blue]ID[/] to Invoice:")
                    .DefaultValue(0)
                    .Validate(id => id == 0 || bookings.Any(b => b.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid ID.[/]")));

            if (bookingId == 0)
                return;

            var selectedBooking = bookings.First(b => b.Id == bookingId);

            if (AnsiConsole.Confirm($"Create invoice for [blue]{selectedBooking.CustomerName}[/] totaling [blue]{selectedBooking.TotalPrice:C}[/]?"))
            {
                bool success = _invoiceService.AddInvoice(selectedBooking.Id);

                if (success)
                    AnsiConsole.MarkupLine("[green]Invoice created successfully![/]");
                else
                    AnsiConsole.MarkupLine("[red]Failed to create invoice.[/]");
            }
        }

        private void ListAllInvoices()
        {
            var invoices = _invoiceService.GetAllInvoices();

            ValidateUI.IsEmpty(invoices, "No invoices found.");

            TableUI.ShowInvoicesTable(invoices, "All Invoices");
        }

        private void UpdateInvoice()
        {
            var invoices = _invoiceService.GetAllInvoices();

            ValidateUI.IsEmpty(invoices, "No invoices available to update.");

            TableUI.ShowInvoicesTable(invoices, "Select Invoice to Update");

            var invoiceId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Invoice [blue]ID[/] to Update:")
                    .DefaultValue(0)
                    .Validate(id => id == 0 || invoices.Any(i => i.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid ID.[/]")));

            if (invoiceId == 0)
                return;

            var invoiceToUpdate = invoices.First(i => i.Id == invoiceId);

            AnsiConsole.MarkupLine($"[yellow]Updating Invoice Id {invoiceToUpdate.Id}[/]");

            invoiceToUpdate.DueDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Enter new [blue]Due Date[/]:")
                    .DefaultValue(invoiceToUpdate.DueDate)
                    .Validate(date => date >= invoiceToUpdate.IssueDate
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Due date cannot be before issue date.[/]")));

            invoiceToUpdate.TotalAmount = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Enter new [blue]Total Amount[/]:")
                    .DefaultValue(invoiceToUpdate.TotalAmount)
                    .Validate(amount => amount >= 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Amount cannot be negative.[/]")));

            invoiceToUpdate.IsPaid = AnsiConsole.Confirm("Is the [blue]Invoice Paid[/]?", invoiceToUpdate.IsPaid);

            if (_validator.IsInvalid(invoiceToUpdate))
                return;

            if (AnsiConsole.Confirm("Confirm update?"))
            {
                if (_invoiceService.UpdateInvoice(invoiceToUpdate))
                    AnsiConsole.MarkupLine("[green]Invoice updated successfully![/]");
            }
        }

        private void DeleteInvoice()
        {
            var invoices = _invoiceService.GetAllInvoices();

            ValidateUI.IsEmpty(invoices, "No invoices found to delete.");

            TableUI.ShowInvoicesTable(invoices, "Select Invoice to Delete");

            var invoiceId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Invoice [blue]ID[/] to Delete:")
                    .DefaultValue(0)
                    .Validate(id => id == 0 || invoices.Any(i => i.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid ID.[/]")));

            if (invoiceId == 0)
                return;

            var invoiceToDelete = invoices.First(i => i.Id == invoiceId);

            AnsiConsole.MarkupLine($"You are about to delete the invoice with ID: [blue]{invoiceToDelete.Id}[/]");
            AnsiConsole.MarkupLine($"Customer: {invoiceToDelete.CustomerName}, Room: {invoiceToDelete.RoomNumber} Issue Date: {invoiceToDelete.IssueDate:yyyy-MM-dd}");

            if (AnsiConsole.Confirm("Are you sure you want to delete Invoice?"))
            {
                if (_invoiceService.DeleteInvoice(invoiceToDelete.Id))
                    AnsiConsole.MarkupLine("[green]Invoice deleted successfully (soft delete).[/]");
                else
                    AnsiConsole.MarkupLine("[red]Could not delete invoice.[/]");
            }
        }

        private void RegisterPayment()
        {
            var unpaidInvoices = _invoiceService.GetUnpaidInvoices();

            ValidateUI.IsEmpty(unpaidInvoices, "No unpaid invoices found.");

            TableUI.ShowInvoicesTable(unpaidInvoices, "Select Unpaid Invoice");

            var invoiceId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Invoice [blue]ID[/] to mark as Paid:")
                    .DefaultValue(0)
                    .Validate(id => id == 0 || unpaidInvoices.Any(i => i.Id == id)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid ID.[/]")));

            if (invoiceId == 0)
                return;

            var selectedInvoice = unpaidInvoices.First(i => i.Id == invoiceId);

            if (AnsiConsole.Confirm($"Register payment for invoice ID {selectedInvoice.Id}?"))
            {
                if (_invoiceService.MarkAsPaid(selectedInvoice.Id))
                    AnsiConsole.MarkupLine("[green]Payment registered![/]");
            }
        }
    }
}
