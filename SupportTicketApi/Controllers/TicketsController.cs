using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApi.Models;
using System.Security.Claims;

namespace SupportTicketApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TicketsController(SupportTicketDbContext context, ILogger<TicketsController> logger) : ControllerBase
{
    // --- DTOs ---
    public record CreateTicketDto(string Subject, string Description, string Priority);
    public record TicketListDto(int TicketId, string Subject, string Priority, string Status, DateTime CreatedDate, string? AssignedToName);
    public record AssignTicketDto(int AdminUserId);
    public record UpdateStatusDto(string NewStatus);
    public record AddCommentDto(string CommentText, bool IsInternal);

    // --- Helpers ---
    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetCurrentUserRole() => User.FindFirstValue(ClaimTypes.Role)!;

    // --- Endpoints ---

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto request)
    {
        try
        {
            var allowedPriorities = new[] { "Low", "Medium", "High" };
            if (!allowedPriorities.Contains(request.Priority))
                return BadRequest(new { Message = "Invalid priority level." });

            var ticket = new Ticket
            {
                Subject = request.Subject,
                Description = request.Description,
                Priority = request.Priority,
                Status = "Open",
                CreatedByUserId = GetCurrentUserId()
            };

            await context.Tickets.AddAsync(ticket);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicketList), new { id = ticket.TicketId }, new { Message = "Ticket created successfully", TicketId = ticket.TicketId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating ticket for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { Message = "An error occurred while creating the ticket." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTicketList()
    {
        try
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var query = context.Tickets.AsNoTracking().AsQueryable();

            if (role == "User")
            {
                query = query.Where(t => t.CreatedByUserId == userId);
            }

            var tickets = await query
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => new TicketListDto(
                    t.TicketId,
                    t.Subject,
                    t.Priority,
                    t.Status,
                    t.CreatedDate ?? DateTime.UtcNow,
                    context.Users.Where(u => u.UserId == t.AssignedToUserId).Select(u => u.Username).FirstOrDefault()
                ))
                .ToListAsync();

            return Ok(tickets);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving ticket list for user {UserId}", GetCurrentUserId());
            return StatusCode(500, new { Message = "An error occurred while retrieving tickets." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketDetails(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var ticket = await context.Tickets
                .AsNoTracking()
                .Where(t => t.TicketId == id)
                .Select(t => new
                {
                    t.TicketId,
                    t.Subject,
                    t.Description,
                    t.Priority,
                    t.Status,
                    t.CreatedDate,
                    t.AssignedToUserId,
                    t.CreatedByUserId,
                    AssignedToName = context.Users.Where(u => u.UserId == t.AssignedToUserId).Select(u => u.Username).FirstOrDefault(),
                    Comments = context.Ticketcomments.Where(c => c.TicketId == id).OrderBy(c => c.CreatedAt).ToList(),
                    History = context.Ticketstatushistory.Where(h => h.TicketId == id).OrderBy(h => h.ChangedAt).ToList()
                })
                .FirstOrDefaultAsync();

            if (ticket == null) return NotFound(new { Message = "Ticket not found." });

            if (role == "User" && ticket.CreatedByUserId != userId)
                return Forbid();

            var filteredComments = role == "User"
                ? ticket.Comments.Where(c => !c.IsInternal).ToList()
                : ticket.Comments;

            return Ok(new { Ticket = ticket, Comments = filteredComments, History = ticket.History });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving details for ticket {TicketId}", id);
            return StatusCode(500, new { Message = "Server error retrieving ticket details." });
        }
    }

    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignTicketDto request)
    {
        if (GetCurrentUserRole() != "Admin") return Forbid();

        var ticket = await context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        ticket.AssignedToUserId = request.AdminUserId;
        await context.SaveChangesAsync();

        return Ok(new { Message = "Ticket assigned successfully." });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateStatusDto request)
    {
        if (GetCurrentUserRole() != "Admin") return Forbid();

        var ticket = await context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        var oldStatus = ticket.Status;
        var newStatus = request.NewStatus;

        bool isValidFlow = (oldStatus == "Open" && newStatus == "In Progress") ||
                           (oldStatus == "In Progress" && newStatus == "Closed");

        if (!isValidFlow)
            return BadRequest(new { Message = $"Invalid status transition from {oldStatus} to {newStatus}." });

        ticket.Status = newStatus;

        var historyLog = new Ticketstatushistory
        {
            TicketId = ticket.TicketId,
            ChangedByUserId = GetCurrentUserId(),
            OldStatus = oldStatus,
            NewStatus = newStatus
        };

        await context.Ticketstatushistory.AddAsync(historyLog);
        await context.SaveChangesAsync();

        return Ok(new { Message = "Status updated and logged." });
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentDto request)
    {
        var role = GetCurrentUserRole();
        var userId = GetCurrentUserId();

        var ticket = await context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        if (role == "User" && ticket.Status == "Closed")
            return BadRequest(new { Message = "Cannot add comments to a closed ticket." });

        if (role == "User" && ticket.CreatedByUserId != userId)
            return Forbid();

        var comment = new Ticketcomment
        {
            TicketId = ticket.TicketId,
            UserId = userId,
            CommentText = request.CommentText,
            IsInternal = role == "Admin" && request.IsInternal
        };

        await context.Ticketcomments.AddAsync(comment);
        await context.SaveChangesAsync();

        return Ok(new { Message = "Comment added successfully." });
    }

    [HttpGet("admins")]
    public async Task<IActionResult> GetAdmins()
    {
        var admins = await context.Users
            .Where(u => u.Role == "Admin")
            .Select(u => new { u.UserId, u.Username })
            .ToListAsync();

        return Ok(admins);
    }
}