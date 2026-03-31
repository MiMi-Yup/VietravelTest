using System.Text;
using Application.Configurations;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Configurations;
using WebAPI.Filters;

namespace WebAPI.Extensions;

public static class ServiceExtension
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
    }

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind JwtConfiguration from config
        var jwtSection = configuration.GetSection("JwtSettings");
        var jwtConfig = jwtSection.Get<JwtConfiguration>() ?? new JwtConfiguration();
        var secretKey = Encoding.UTF8.GetBytes(jwtConfig.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped<IJwtService, JwtService>();
    }

    public static void ConfigureOption(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind all configuration classes via IOptions<T>
        services.Configure<IPWhitelistConfiguration>(configuration.GetSection("IPWhitelist"));
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
        services.Configure<AuthConfiguration>(configuration.GetSection("AuthSettings"));
    }

    public static void ConfigureApp(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<IBookingRequestRepository, BookingRequestRepository>();

        // Services
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IBookingRequestService, BookingRequestService>();

        // Filters
        services.AddScoped<IpWhitelistFilter>();

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();
    }
}