
namespace Domain.Entities
{
    public class City
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string PostOfficeCode { get; set; }
        public IList<Hotel> Hotels { get; set; }

    }
}
