using IssueTracker.Api.DTOs;
using IssueTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers;

[ApiController]
[Route("tickets")]
public sealed class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetTickets(CancellationToken cancellationToken)
    {
        var tickets = await ticketService.GetTickets(cancellationToken);
        return Ok(tickets);
    }
}
