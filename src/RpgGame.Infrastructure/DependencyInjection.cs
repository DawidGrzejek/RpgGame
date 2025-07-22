using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Domain.Interfaces.Repositories;
using RpgGame.Infrastructure.EventStore;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.Repositories;
using RpgGame.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Authentication;
using RpgGame.Infrastructure.Services.Authentication;
using RpgGame.Application.Interfaces.Persistence;


namespace RpgGame.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure services in the DI container
    /// This is where Infrastructure layer implementations are registered for Application layer interfaces
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<GameDbContext>();

            // Add ASP.NET Core Identity services
            services.AddDbContext<IdentityDataContext>();
            
            // Configure ASP.NET Core Identity
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 12;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false; // For simplicity

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<IdentityDataContext>()
            .AddDefaultTokenProviders();
            
            // Configure JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT Secret is not configured in appsettings.json (JwtSettings:Secret)");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                // SignalR support
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/gameHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Configure Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Player", policy => policy.RequireRole("Player"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Moderator", policy => policy.RequireRole("Admin", "Moderator"));
            });

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register event store repository
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();

            // Register domain repositories
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<IGameSaveRepository, GameSaveRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IEnemyTemplateRepository, EnemyTemplateRepository>();
            services.AddScoped<IItemTemplateRepository, ItemTemplateRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register authentication services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}