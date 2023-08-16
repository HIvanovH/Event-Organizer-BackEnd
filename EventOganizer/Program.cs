using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using EventOganizer.Context;
using Jose;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using EventOganizer.JWT;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json");
        var configuration = builder.Configuration;

        // Read JWT settings from configuration
        var jwtSettings = configuration.GetSection("Jwt").Get<EventOganizer.JWT.JwtSettings >();

        builder.Services.Configure<EventOganizer.JWT.JwtSettings>(configuration.GetSection("Jwt"));

        builder.Services.AddDbContext<AplicationDBContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("local");
            options.UseSqlServer(connectionString);
        });

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // Configure identity options here
        })
        .AddEntityFrameworkStores<AplicationDBContext>()
        .AddDefaultTokenProviders();

        var serviceProvider = builder.Services.BuildServiceProvider();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
        }

        if (!roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();
        }

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        // Add JWT authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = serviceProvider.GetRequiredService<IOptions<EventOganizer.JWT.JwtSettings>>().Value;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        })
        .AddCookie();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
