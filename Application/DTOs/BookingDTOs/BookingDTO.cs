using Application.DTOs.ReviewDTOs;
using Domain.Enum;

public record BookingDTO
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public DateTime BookingDate { get; set; }
    public double Price { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    // Include review details if available
    public ReviewDTO? Review { get; set; }
}