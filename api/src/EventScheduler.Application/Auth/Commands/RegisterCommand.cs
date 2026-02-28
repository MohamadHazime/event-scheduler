using EventScheduler.Application.Auth.DTOs;
using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.Interfaces;
using EventScheduler.Domain.Users;
using EventScheduler.Domain.Users.Interfaces;
using MediatR;

namespace EventScheduler.Application.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            return Result<AuthResponse>.Failure("A user with this email already exists.", 409);

        var user = new User
        {
            Email = request.Email.ToLower().Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            FullName = request.FullName.Trim()
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user);
        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.CreatedAt);

        return Result<AuthResponse>.Success(new AuthResponse(token, userDto), 201);
    }
}