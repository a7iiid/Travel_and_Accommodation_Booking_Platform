using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RoomDTOs
{
    public record RoomDTOForAvailableRooms
    {
        public Guid Id { get; set; }
        public Guid RoomTypeId { get; set; }
        public int Capacity { get; set; }

        public float Rating { get; set; }

        public RoomType RoomType { get; set; }
    }
}
