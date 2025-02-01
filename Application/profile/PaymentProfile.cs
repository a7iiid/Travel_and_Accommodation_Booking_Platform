using Application.DTOs.PaymentDTOs;
using AutoMapper;
using Domain.Entities;


namespace Application.profile
{
    public class PaymentProfile:Profile
    {
        public PaymentProfile()
        {
            CreateMap<PaymentDTO,Payment >()
                .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap();
            
        }
    }
}
