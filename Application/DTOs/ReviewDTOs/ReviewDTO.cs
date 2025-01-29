

namespace Application.DTOs.ReviewDTOs
{
    public record ReviewDTO
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public float Rating { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
