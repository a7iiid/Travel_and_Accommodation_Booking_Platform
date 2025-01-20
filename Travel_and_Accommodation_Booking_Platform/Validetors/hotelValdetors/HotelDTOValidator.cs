using Application.DTOs.HotelDTOs;
using FluentValidation;

namespace Application.Validators
{
    public class HotelDTOValidator : AbstractValidator<HotelDTO>
    {
        public HotelDTOValidator()
        {
            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("City ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Hotel name is required.")
                .MaximumLength(100).WithMessage("Hotel name must not exceed 100 characters.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.");

            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(200).WithMessage("Street address must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.FloorsNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Floors number must be at least 1.");
        }
    }
}
