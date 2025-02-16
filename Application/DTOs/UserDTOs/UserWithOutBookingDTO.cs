

namespace Application.DTOs.UserDTOs
{
    public record UserWithOutBookingDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public bool isAdmin { get; set; }
    }
}
