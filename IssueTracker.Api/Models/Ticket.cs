namespace IssueTracker.Api.Models;

public sealed class Ticket
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string Status { get; set; } = "Öppet";
    public string Priority { get; set; } = "Normal";
}
