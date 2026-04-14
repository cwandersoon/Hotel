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
            int expiredCount = _bookingService.DeleteExpiredBookings();

            if (expiredCount > 0)
                AnsiConsole.MarkupLine($"[yellow]{expiredCount} Bookings with unpaid Invoices are cancelled.[/]");

            while (true)
            {
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

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]No bookings available to invoice.[/]");
                return;
            }

            TableUI.ShowBookingsTable(bookings, "Bookings available for Invoice");

            var bookingId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Booking [blue]ID[/] to create Invoice:")
                    .DefaultValue(0));

            if (bookingId == 0)
                return;

            var selectedBooking = bookings.FirstOrDefault(b => b.Id == bookingId);

            if (selectedBooking == null)
            {
                AnsiConsole.MarkupLine("[red]Invalid Booking ID.[/]");
                return;
            }

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

            if (!invoices.Any())
            {
                AnsiConsole.MarkupLine("[red]No invoices found.[/]");
                return;
            }

            TableUI.ShowInvoicesTable(invoices, "All Invoices");
        }

        private void UpdateInvoice()
        {
            var invoices = _invoiceService.GetAllInvoices();

            if (!invoices.Any())
            {
                AnsiConsole.MarkupLine("[red]No invoices available to update.[/]");
                return;
            }

            TableUI.ShowInvoicesTable(invoices, "Select Invoice to Update");

            var invoiceId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Invoice [blue]ID[/] to Update:")
                    .DefaultValue(0));

            if (invoiceId == 0)
                return;

            var invoiceToUpdate = invoices.FirstOrDefault(i => i.Id == invoiceId);

            if (invoiceToUpdate == null)
            {
                AnsiConsole.MarkupLine("[red]Invoice not found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Updating Invoice Id {invoiceToUpdate.Id}[/]");

            invoiceToUpdate.DueDate = AnsiConsole.Prompt(
                new TextPrompt<DateTime>($"Enter new [blue]Due Date[/]:")
                    .DefaultValue(invoiceToUpdate.DueDate));

            invoiceToUpdate.TotalAmount = AnsiConsole.Prompt(
                new TextPrompt<decimal>($"Enter new [blue]Total Amount[/]:")
                    .DefaultValue(invoiceToUpdate.TotalAmount));

            invoiceToUpdate.IsPaid = AnsiConsole.Confirm("Is the [blue]Invoice Paid[/]?", invoiceToUpdate.IsPaid);

            if (_validator.IsInvalid(invoiceToUpdate))
                return;

            if (AnsiConsole.Confirm("Confirm update?"))
            {
                if (_invoiceService.UpdateInvoice(invoiceToUpdate))
                    AnsiConsole.MarkupLine("[green]Invoice updated successfully![/]");
                else
                    AnsiConsole.MarkupLine("[red]Failed to update invoice in database.[/]");
            }
        }

        private void DeleteInvoice()
        {
            var invoices = _invoiceService.GetAllInvoices();

            if (!invoices.Any())
            {
                AnsiConsole.MarkupLine("[red]No invoices found.[/]");
                return;
            }

            TableUI.ShowInvoicesTable(invoices, "Select Invoice to Delete");

            var invoiceId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Invoice [blue]ID[/] to Delete:")
                    .DefaultValue(0));

            if (invoiceId == 0)
                return;

            var invoiceToDelete = invoices.FirstOrDefault(i => i.Id == invoiceId);

            if (invoiceToDelete == null)
            {
                AnsiConsole.MarkupLine("[red]Invoice not found.[/]");
                return;
            }

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

            if (!unpaidInvoices.Any())
            {
                AnsiConsole.MarkupLine("[red]No unpaid invices found.[/]");
                return;
            }

            TableUI.ShowInvoicesTable(unpaidInvoices, "Select Unpaid Invoice");

            var invoiceId = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter Invoice [blue]ID[/] to mark as Paid:")
                    .DefaultValue(0));

            if (invoiceId == 0)
                return;

            var selectedInvoice = unpaidInvoices.FirstOrDefault(i => i.Id == invoiceId);

            if (selectedInvoice == null)
            {
                AnsiConsole.MarkupLine("[red]Invoice not found.[/]");
                return;
            }

            if (AnsiConsole.Confirm($"Register payment for invoice ID {selectedInvoice.Id}?"))
            {
                if (_invoiceService.MarkAsPaid(selectedInvoice.Id))
                    AnsiConsole.MarkupLine("[green]Payment registered![/]");
            }
        }
    }
}
