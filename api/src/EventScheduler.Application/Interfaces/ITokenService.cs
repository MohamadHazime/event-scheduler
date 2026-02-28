using EventScheduler.Domain.Users;

namespace EventScheduler.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}