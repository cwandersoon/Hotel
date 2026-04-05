using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface IInvoiceService
    {
        InvoiceDTO AddInvoiceForBooking(int bookingId);
        List<InvoiceDTO> GetAllInvoices();
        List<InvoiceDTO> GetUnpaidInvoices();
        InvoiceDTO? GetInvoiceByBookingId(int bookingId);
        bool UpdateInvoice(InvoiceDTO invoiceDto);
        bool DeleteInvoice(int invoiceId);


        bool MarkAsPaid(int invoiceId);
        decimal CalculateTax(int invoiceId);
        bool CheckIfOverdue(int invoiceId);
        void SendReminder(int invoiceId);
    }
}
