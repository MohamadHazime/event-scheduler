using EventScheduler.Application.Auth.DTOs;
using EventScheduler.Application.Common;
using EventScheduler.Application.Interfaces;
using EventScheduler.Domain.Users.Interfaces;
using MediatR;

namespace EventScheduler.Application.Auth.Queries;

public record GetCurrentUserQuery : IRequest<Result<UserDto>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<UserDto>.Failure("Unauthorized.", 401);

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user is null)
            return Result<UserDto>.Failure("User not found.", 404);

        return Result<UserDto>.Success(new UserDto(user.Id, user.Email, user.FullName, user.CreatedAt));
    }
}