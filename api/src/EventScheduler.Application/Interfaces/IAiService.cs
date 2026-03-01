namespace EventScheduler.Application.Interfaces;

public interface IAiService
{
    Task<string> GenerateContentAsync(string prompt);
}