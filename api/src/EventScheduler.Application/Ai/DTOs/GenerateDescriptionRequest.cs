namespace EventScheduler.Application.Ai.DTOs;

public record GenerateDescriptionRequest(string Title, string? Category, string? Location);