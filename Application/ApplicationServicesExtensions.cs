using Application.profile;

using Microsoft.Extensions.DependencyInjection;


namespace Application
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationCollection(this IServiceCollection services)
        {
          


            services.AddAutoMapper(typeof(CityProfile));
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(HotelProfile));
            services.AddAutoMapper(typeof(RoomProfile));
            services.AddAutoMapper(typeof(BookingProfile));


            return services;
        }
    }
}
