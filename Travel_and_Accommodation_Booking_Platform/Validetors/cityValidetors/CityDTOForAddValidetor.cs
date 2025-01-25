using Application.DTOs.CityDTOs;
using FluentValidation;

namespace Presentation.Validetors.cityValidetors
{
    public class CityDTOForAddValidetor: AbstractValidator<CityDTOForAdd>
    {
        public CityDTOForAddValidetor()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("City name is required.")
               .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");

            RuleFor(x => x.CountryName)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.");

            RuleFor(x => x.CountryCode)
                .NotEmpty().WithMessage("Country code is required.")
                .Length(2).WithMessage("Country code must be exactly 2 characters.");

            RuleFor(x => x.PostOffice)
                .NotEmpty().WithMessage("Post office code is required.")
                .MaximumLength(20).WithMessage("Post office code must not exceed 20 characters.");

        }
    }
}
