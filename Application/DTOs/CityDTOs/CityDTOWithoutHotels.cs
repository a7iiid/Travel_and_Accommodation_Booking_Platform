

namespace Application.DTOs.CityDTOs
{
    public record CityDTOWithoutHotels
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string PostOffice { get; set; } = string.Empty;
    }
}
