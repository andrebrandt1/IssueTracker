using Dapper;
using IssueTracker.Api.DTOs;
using IssueTracker.Api.Models;
using Microsoft.Data.SqlClient;

namespace IssueTracker.Api.Services;

public sealed class TicketService(string connectionString) : ITicketService
{
    public async Task<IReadOnlyList<TicketDto>> GetTickets(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT [Id], [Title], [Description], [Status], [Priority]
            FROM [dbo].[Tickets]
            ORDER BY [Id];
            """;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var tickets = await connection.QueryAsync<Ticket>(command);

        return tickets.Select(ticket => new TicketDto(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status,
                ticket.Priority))
            .ToList();
    }
}
