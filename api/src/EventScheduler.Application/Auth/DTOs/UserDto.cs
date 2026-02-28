namespace EventScheduler.Application.Auth.DTOs;

public record UserDto(Guid Id, string Email, string FullName, DateTime CreatedAt);