

namespace Domain.Model
{
    public record HotelSearchParameters
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string? CityName { get; set; }
        public float StarRate { get; set; }
        public int Capacity { get; set; }
        
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
