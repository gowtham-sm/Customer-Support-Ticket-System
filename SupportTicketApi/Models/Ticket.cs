using System;
using System.Collections.Generic;

namespace SupportTicketApi.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int CreatedByUserId { get; set; }

    public int? AssignedToUserId { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<Ticketcomment> Ticketcomments { get; set; } = new List<Ticketcomment>();

    public virtual ICollection<Ticketstatushistory> Ticketstatushistories { get; set; } = new List<Ticketstatushistory>();
}
