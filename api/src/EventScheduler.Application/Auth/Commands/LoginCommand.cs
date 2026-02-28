using EventScheduler.Application.Auth.DTOs;
using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.Users.Interfaces;
using MediatR;

namespace EventScheduler.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLower().Trim());

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid email or password.", 401);

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.CreatedAt);

        return Result<AuthResponse>.Success(new AuthResponse(token, userDto));
    }
}