namespace EventScheduler.Application.Auth.DTOs;

public record AuthResponse(string Token, UserDto User);