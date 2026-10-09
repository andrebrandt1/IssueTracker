using IssueTracker.Api.DTOs;

namespace IssueTracker.Api.Services;

public interface ITicketService
{
    Task<IReadOnlyList<TicketDto>> GetTickets(CancellationToken cancellationToken = default);
    Task<TicketDto> CreateTicket(CreateTicketDto ticket, CancellationToken cancellationToken = default);
    Task<bool> DeleteTicket(int id, CancellationToken cancellationToken = default);
}
