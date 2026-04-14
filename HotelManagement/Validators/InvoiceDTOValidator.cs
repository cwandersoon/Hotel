using FluentValidation;
using HotelManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Validators
{
    public class InvoiceDTOValidator : AbstractValidator<InvoiceDTO>
    {
        public InvoiceDTOValidator()
        {
            RuleFor(i => i.TotalAmount)
                .NotEmpty().WithMessage("Total amount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative.")
                .LessThan(10000000).WithMessage("Total amount is too large.");

            RuleFor(i => i.IssueDate)
                .NotEmpty().WithMessage("Issue date is required.");

            RuleFor(i => i.DueDate)
                .NotEmpty().WithMessage("Due date is required.")
                .GreaterThanOrEqualTo(i => i.IssueDate)
                .WithMessage("Invalid due date.");

            RuleFor(i => i.BookingId)
                .NotEmpty().WithMessage("An invoice must be linked to a booking.");
        }
    }
}
