using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.EventAttendees.Interfaces;
using EventScheduler.Domain.Events.Interfaces;
using EventScheduler.Domain.Interfaces;
using EventScheduler.Domain.Invitations.Interfaces;
using EventScheduler.Domain.Users.Interfaces;
using EventScheduler.Infrastructure.Data;
using EventScheduler.Infrastructure.Repositories;
using EventScheduler.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventScheduler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventAttendeeRepository, EventAttendeeRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}