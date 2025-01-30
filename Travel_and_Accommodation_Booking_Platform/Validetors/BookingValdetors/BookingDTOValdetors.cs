using Application.DTOs.BookingDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class BookingDTOValidator : AbstractValidator<BookingDTO>
    {
        public BookingDTOValidator()
        {
            // Validate UserId is not empty
            RuleFor(booking => booking.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");

            // Validate CheckInDate is in the future
            RuleFor(booking => booking.CheckInDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Check-in date must be in the future.");

            // Validate CheckOutDate is after CheckInDate
            RuleFor(booking => booking.CheckOutDate)
                .GreaterThan(booking => booking.CheckInDate)
                .WithMessage("Check-out date must be after check-in date.");

            // Validate BookingDate is not in the future (should represent when the booking was made)
            RuleFor(booking => booking.BookingDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Booking date cannot be in the future.");

            // Validate Price is positive
            RuleFor(booking => booking.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            
        }
    }
}
