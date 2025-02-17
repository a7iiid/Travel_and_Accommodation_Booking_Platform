using System.Reflection;
using System.Text;
using Application.@interface;
using Application.profile;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Auth.password;
using Infrastructure.DB;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pay.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBContext")));


services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = false;
}).AddNewtonsoftJson()
  .AddXmlDataContractSerializerFormatters();

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
    setupAction.AddSecurityDefinition("TAABPApiAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "TAABPApiAuth"}
            },
            new List<string>()
        }
    });
});



services.AddAuthentication("Bearer").AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecretKey"]))
        };
    });

services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
});

services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
//roll 
services.AddAuthorization(options =>
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("isAdmin", "True");
    }));

services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IRepository<User>, UserRepository>();
services.AddScoped<IAuthUser, AuthUser>();
services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
services.AddScoped<UserService>();
services.AddScoped<ICityRepository, CityRepository>();
services.AddScoped<CityServices>();

services.AddScoped<IHotelRepository, HotelRepository>();
services.AddScoped<HotelRepository>();
services.AddScoped<HotelServices>();

services.AddScoped<IRepository<Room>, RoomRepository>();
services.AddScoped<RoomRepository>();

services.AddScoped<IBookingRepository, BookingRepository>();
services.AddScoped<BookingRepository>();
services.AddScoped<BookingServices>();

services.AddScoped<IPaymentServices, PaymentServices>();
builder.Services.AddScoped<PaymentServices>();


builder.Services.AddScoped<PayPalService>();
builder.Services.AddScoped<IPayment, PayPalService>();



builder.Services.AddAutoMapper(typeof(CityProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(HotelProfile));
builder.Services.AddAutoMapper(typeof(RoomProfile));
builder.Services.AddAutoMapper(typeof(BookingProfile));





var app = builder.Build();
app.UseAuthentication();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();