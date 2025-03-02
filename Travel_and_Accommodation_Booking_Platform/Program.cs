using System.Reflection;
using System.Text;
using Application;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Auth.password;
using Infrastructure.DB;
using Infrastructure.EmailService;
using Infrastructure.Invoice;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pay.Interfaces;
using QuestPDF.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBContext")));
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));


services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = false;
}).AddNewtonsoftJson(
    options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
  .AddXmlDataContractSerializerFormatters();



services.AddEndpointsApiExplorer();

services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
   
});



services.AddAuthentication("Bearer").AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("Issuer"),
            ValidAudience = Environment.GetEnvironmentVariable("Audience"),
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SecretKey")))
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

services.AddApplicationAutoMapper();

QuestPDF.Settings.License = LicenseType.Community;

services.AddTransient<IPasswordHasher, PasswordHasher>();
services.AddScoped<IUserRepository, UserRepository>();
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
services.AddTransient<IEmailSender, EmailSender>();

services.AddTransient<IInvoice, Invoice>();

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