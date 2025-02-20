using Application.profile;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Auth.password;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Pay.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationCollection(this IServiceCollection services)
        {
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthUser, AuthUser>();
            services.AddTransient<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<UserService>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<CityServices>();

            services.AddScoped<HotelRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<HotelServices>();

            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<RoomRepository>();

            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<BookingRepository>();
            services.AddScoped<BookingServices>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<PaymentRepository>();

            services.AddScoped<PaymentServices>();


            services.AddScoped<PayPalGateWay>();
            services.AddScoped<IPaymentGateway, PayPalGateWay>();



            services.AddAutoMapper(typeof(CityProfile));
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(HotelProfile));
            services.AddAutoMapper(typeof(RoomProfile));
            services.AddAutoMapper(typeof(BookingProfile));


            return services;
        }
    }
}
