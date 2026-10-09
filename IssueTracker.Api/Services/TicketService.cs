using Dapper;
using IssueTracker.Api.DTOs;
using IssueTracker.Api.Models;
using Microsoft.Data.SqlClient;

namespace IssueTracker.Api.Services;

public sealed class TicketService(string connectionString) : ITicketService
{
    public async Task<bool> DeleteTicket(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM [dbo].[Tickets]
            WHERE [Id] = @Id;
            """;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        var affectedRows = await connection.ExecuteAsync(command);
        return affectedRows > 0;
    }

    public async Task<TicketDto> CreateTicket(CreateTicketDto ticket, CancellationToken cancellationToken = default)
    {
        var databasePriority = ticket.Priority switch
        {
            "Low" => "Låg",
            "Normal" => "Normal",
            "High" => "Hög",
            _ => throw new ArgumentException("Prioritet måste vara Low, Normal eller High.", nameof(ticket))
        };

        const string sql = """
            INSERT INTO [dbo].[Tickets] ([Title], [Description], [Status], [Priority])
            OUTPUT INSERTED.[Id], INSERTED.[Title], INSERTED.[Description],
                   INSERTED.[Status], INSERTED.[Priority]
            VALUES (@Title, @Description, N'Öppet', @Priority);
            """;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(sql, new
        {
            ticket.Title,
            ticket.Description,
            Priority = databasePriority
        }, cancellationToken: cancellationToken);

        var createdTicket = await connection.QuerySingleAsync<Ticket>(command);

        return new TicketDto(createdTicket.Id, createdTicket.Title, createdTicket.Description,
            "Open", ticket.Priority);
    }

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
