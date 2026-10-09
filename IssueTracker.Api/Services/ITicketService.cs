using IssueTracker.Api.DTOs;

namespace IssueTracker.Api.Services;

public interface ITicketService
{
    Task<IReadOnlyList<TicketDto>> GetTickets(CancellationToken cancellationToken = default);
}
