using FluentValidation;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Validators
{
    public class CustomerDTOValidator : AbstractValidator<CustomerDTO>
    {
        readonly ICustomerService _customerService;

        public CustomerDTOValidator(ICustomerService customerService)
        {
            _customerService = customerService;

            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
                .Matches(@"^[\p{L}\s'-]+$").WithMessage("First name contains invalid characters.");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
                .Matches(@"^[\p{L}\s'-]+$").WithMessage("Last name contains invalid characters.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(c => c.Email)
                .Must((dto, email) =>
                {
                    if (dto.Id == 0)
                        return _customerService.IsEmailUnique(email);

                    var existingCustomer = _customerService.GetCustomerById(dto.Id);

                    return _customerService.IsEmailUnique(email) || (existingCustomer != null && existingCustomer.Email == email);
                }).WithMessage("This email is already in use by another customer.");

            RuleFor(c => c.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
                .Matches(@"^[\d\+\-\s]+$").WithMessage("Phone number contains invalid characters.");

            RuleFor(c => c.StreetAddress)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(100);

            RuleFor(c => c.ZipCode)
                .NotEmpty().WithMessage("Zip code is required.")
                .MaximumLength(10).WithMessage("Zip code is too long.")
                .Matches(@"^[\d\s]+$").WithMessage("Zip code contains invalid characters.");

            RuleFor(c => c.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50);
        }
    }
}
