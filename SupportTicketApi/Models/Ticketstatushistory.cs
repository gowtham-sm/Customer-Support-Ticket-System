using System;
using System.Collections.Generic;

namespace SupportTicketApi.Models;

public partial class Ticketstatushistory
{
    public int HistoryId { get; set; }

    public int TicketId { get; set; }

    public int ChangedByUserId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = null!;

    public DateTime? ChangedAt { get; set; }

    public virtual User ChangedByUser { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
