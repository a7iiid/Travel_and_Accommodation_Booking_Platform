using Application.DTOs;
using Application.DTOs;
using Application.DTOs.RoomDTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.profile
{
    public class RoomProfile:Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDTO>();
            CreateMap<RoomDTO, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); ;
        }
    }
}
