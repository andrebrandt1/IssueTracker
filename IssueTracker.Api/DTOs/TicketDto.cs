namespace IssueTracker.Api.DTOs;

public sealed record TicketDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority);
