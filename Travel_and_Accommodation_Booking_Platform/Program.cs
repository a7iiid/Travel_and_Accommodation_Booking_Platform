using System.Reflection;
using System.Text;
using Application.profile;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DB;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

services.AddAuthorization(options =>
    options.AddPolicy("MustBeAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("Role", "Admin");
    }));
services.AddScoped<Infrastructure.Auth.password.IPasswordHasher, Infrastructure.Auth.password.PasswordHasher>();

services.AddScoped<Infrastructure.Auth.IAuthUser, Infrastructure.Auth.AuthUser>();

services.AddScoped<Infrastructure.Auth.ITokenGenerator, Infrastructure.Auth.JwtTokenGenerator>();
services.AddScoped<Domain.Interfaces.IRepository<User>, Infrastructure.Repository.Repository<User>>();

builder.Services.AddAutoMapper(typeof(UserProfile));



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