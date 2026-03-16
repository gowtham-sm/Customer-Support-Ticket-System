using System;
using System.Collections.Generic;

namespace SupportTicketApi.Models;

public partial class Ticketcomment
{
    public int CommentId { get; set; }

    public int TicketId { get; set; }

    public int UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public bool IsInternal { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
