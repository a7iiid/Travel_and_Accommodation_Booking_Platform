using Application.DTOs.HotelDTOs;

namespace Application.DTOs.CityDTOs
{
    public record CityDTO
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? PostOfficeCode { get; set; }
        public List<HotelDTO>? Hotels { get; set; }
    }
}
