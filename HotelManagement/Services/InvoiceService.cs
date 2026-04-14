using AutoMapper;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace HotelManagement.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public InvoiceService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public bool AddInvoice(int bookingId)
        {
            if (_dbContext.Invoices.Any(i => i.BookingId == bookingId))
                return false;

            var booking = _dbContext.Bookings.Find(bookingId);

            if (booking == null)
                return false;

            var invoice = new Invoice
            {
                BookingId = bookingId,
                TotalAmount = booking.TotalPrice,
                IssueDate = booking.CreatedAt,
                DueDate = booking.CreatedAt.AddDays(10),
                IsPaid = false,
            };

            _dbContext.Invoices.Add(invoice);
            _dbContext.SaveChanges();

            return true;
        }

        public List<InvoiceDTO> GetAllInvoices()
        {
            var invoices = _dbContext.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Room)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .ToList();
            return _mapper.Map<List<InvoiceDTO>>(invoices);
        }

        public List<InvoiceDTO> GetUnpaidInvoices()
        {
            var unpaid = _dbContext.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Where(i => !i.IsPaid)
                .ToList();
            return _mapper.Map<List<InvoiceDTO>>(unpaid);
        }

        public InvoiceDTO GetInvoiceById(int invoiceId)
        {
            var invoice = _dbContext.Invoices.Find(invoiceId);
            return _mapper.Map<InvoiceDTO>(invoice);
        }

        public InvoiceDTO GetInvoiceByBookingId(int bookingId)
        {
            var invoice = _dbContext.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Room)
                .FirstOrDefault(i => i.BookingId == bookingId);
            return _mapper.Map<InvoiceDTO>(invoice);
        }

        public bool UpdateInvoice(InvoiceDTO invoiceDto)
        {
            var invoice = _dbContext.Invoices.FirstOrDefault(i => i.Id == invoiceDto.Id);

            if (invoice == null)
                return false;

            _mapper.Map(invoiceDto, invoice);

            _dbContext.SaveChanges();

            return true;
        }

        public bool DeleteInvoice(int invoiceId)
        {
            var invoice = _dbContext.Invoices.Find(invoiceId);

            if (invoice == null)
                return false;

            if (invoice.IsPaid)
                return false;

            invoice.IsDeleted = true;

            _dbContext.SaveChanges();

            return true;
        }

        public bool MarkAsPaid(int invoiceId)
        {
            var invoice = _dbContext.Invoices.Find(invoiceId);

            if (invoice == null || invoice.IsPaid)
                return false;

            invoice.IsPaid = true;

            _dbContext.SaveChanges();

            return true;
        }
    }
}
