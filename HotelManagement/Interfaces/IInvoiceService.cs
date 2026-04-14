using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface IInvoiceService
    {
        bool AddInvoice(int bookingId);
        List<InvoiceDTO> GetAllInvoices();
        List<InvoiceDTO> GetUnpaidInvoices();
        InvoiceDTO GetInvoiceById(int invoiceId);
        InvoiceDTO GetInvoiceByBookingId(int bookingId);
        bool UpdateInvoice(InvoiceDTO invoiceDto);
        bool DeleteInvoice(int invoiceId);
        bool MarkAsPaid(int invoiceId);
    }
}
