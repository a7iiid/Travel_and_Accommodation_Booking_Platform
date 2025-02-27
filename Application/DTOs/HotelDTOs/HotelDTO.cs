﻿using Application.DTOs.CityDTOs;
using Domain.Entities;

namespace Application.DTOs.HotelDTOs
{
    public record HotelDTO
    {

        public string Name { get; set; }
        public float Rating { get; set; }
        public string StreetAddress { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public int FloorsNumber { get; set; }
        public Guid OwnerId { get; set; }
        public Guid CityId { get; set; }
    }
}
