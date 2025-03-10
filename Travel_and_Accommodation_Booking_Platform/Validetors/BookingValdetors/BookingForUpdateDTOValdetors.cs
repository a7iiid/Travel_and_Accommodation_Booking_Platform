﻿using Application.DTOs.BookingDTOs;
using FluentValidation;

namespace Presentation.Validetors.BookingValdetors
{
    public class BookingForUpdateDTOValdetors: AbstractValidator<BookingForUpdateDTO>
    {
        public BookingForUpdateDTOValdetors()
        {
            // Rule for RoomId
            RuleFor(booking => booking.RoomId)
                .NotEmpty().WithMessage("RoomId is required.")
                .NotEqual(Guid.Empty).WithMessage("RoomId must be a valid GUID.");



            // Rule for CheckInDate
            RuleFor(booking => booking.CheckInDate)
                .NotEmpty().WithMessage("CheckInDate is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("CheckInDate must be today or in the future.");

            // Rule for CheckOutDate
            RuleFor(booking => booking.CheckOutDate)
                .NotEmpty().WithMessage("CheckOutDate is required.")
                .GreaterThan(booking => booking.CheckInDate).WithMessage("CheckOutDate must be after CheckInDate.");

        }
    }
}
