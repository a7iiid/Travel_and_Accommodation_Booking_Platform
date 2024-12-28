
namespace Domain.Entities
{
    public class City
    {
        public Guid CityID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int PostOfficeCode { get; set; }
        public IList<Hotel> Hotels { get; set; }

    }
}
