using IssueTracker.Api.DTOs;
using IssueTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers;

[ApiController]
[Route("tickets")]
public sealed class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpDelete("/api/tickets/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTicket(int id, CancellationToken cancellationToken)
    {
        var deleted = await ticketService.DeleteTicket(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("/api/tickets")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketDto>> CreateTicket(
        [FromBody] CreateTicketDto ticket, CancellationToken cancellationToken)
    {
        var createdTicket = await ticketService.CreateTicket(ticket, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, createdTicket);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> GetTickets(CancellationToken cancellationToken)
    {
        var tickets = await ticketService.GetTickets(cancellationToken);
        return Ok(tickets);
    }
}
