using System.Transactions;
using Dapper;
using IssueTracker.Api.DTOs;
using IssueTracker.Api.Models;
using IssueTracker.Api.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace IssueTracker.Api.Tests;

public sealed class TicketServiceDatabaseTests
{
    // Körs endast när databastester uttryckligen väljs. Testärendena rullas tillbaka.
    [Theory(Explicit = true)]
    [InlineData("Low", "Låg")]
    [InlineData("Normal", "Normal")]
    [InlineData("High", "Hög")]
    public async Task Create_persists_swedish_values_and_returns_english_values(
        string apiPriority, string databasePriority)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TicketService>()
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("IssueTracker")
            ?? throw new InvalidOperationException("Konfigurera ConnectionStrings:IssueTracker för databastesterna.");
        var service = new TicketService(connectionString);
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = new CreateTicketDto
        {
            Title = "Test ' ; -- " + Guid.NewGuid().ToString("N"),
            Description = "Användaren kan inte logga in. Åäö och apostrof: ' ska bevaras.",
            Priority = apiPriority
        };
        int createdId;

        using (var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled))
        {
            var created = await service.CreateTicket(request, cancellationToken);
            var second = await service.CreateTicket(request, cancellationToken);
            createdId = created.Id;

            Assert.True(created.Id > 0);
            Assert.NotEqual(created.Id, second.Id);
            Assert.Equal("Open", created.Status);
            Assert.Equal(apiPriority, created.Priority);
            Assert.Equal(request.Title, created.Title);
            Assert.Equal(request.Description, created.Description);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            var stored = await connection.QuerySingleAsync<Ticket>(new CommandDefinition("""
                SELECT [Id], [Title], [Description], [Status], [Priority]
                FROM [dbo].[Tickets]
                WHERE [Id] = @Id;
                """, new { Id = created.Id }, cancellationToken: cancellationToken));

            Assert.Equal("Öppet", stored.Status);
            Assert.Equal(databasePriority, stored.Priority);
            Assert.Equal(request.Title, stored.Title);
            Assert.Equal(request.Description, stored.Description);
            // Ingen Complete(): alla testinserts återställs när transaktionen avslutas.
        }

        var ticketsAfterRollback = await service.GetTickets(cancellationToken);
        Assert.DoesNotContain(ticketsAfterRollback, ticket => ticket.Id == createdId);
    }
}
