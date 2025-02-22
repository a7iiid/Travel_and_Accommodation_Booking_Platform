

using Domain.Entities;

namespace Application.DTOs;

public record RoomDTO
{
    public Guid RoomTypeId { get; set; }
    public int Capacity { get; set; }

    public float Rating { get; set; }

    public RoomType RoomType { get; set; }
}
