using System;
using System.Collections.Generic;

namespace SupportTicketApi.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Ticket> TicketAssignedToUsers { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketCreatedByUsers { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticketcomment> Ticketcomments { get; set; } = new List<Ticketcomment>();

    public virtual ICollection<Ticketstatushistory> Ticketstatushistories { get; set; } = new List<Ticketstatushistory>();
}
