using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace SupportTicketApi.Models;

public partial class SupportTicketDbContext : DbContext
{
    public SupportTicketDbContext()
    {
    }

    public SupportTicketDbContext(DbContextOptions<SupportTicketDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Ticketcomment> Ticketcomments { get; set; }

    public virtual DbSet<Ticketstatushistory> Ticketstatushistory { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PRIMARY");

            entity.ToTable("tickets");

            entity.HasIndex(e => e.AssignedToUserId, "AssignedToUserId");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Priority).HasColumnType("enum('Low','Medium','High')");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Open'")
                .HasColumnType("enum('Open','In Progress','Closed')");
            entity.Property(e => e.Subject).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.TicketAssignedToUsers)
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("tickets_ibfk_2");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.TicketCreatedByUsers)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_ibfk_1");
        });

        modelBuilder.Entity<Ticketcomment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("ticketcomments");

            entity.HasIndex(e => e.TicketId, "TicketId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.CommentText).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Ticketcomments)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("ticketcomments_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Ticketcomments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticketcomments_ibfk_2");
        });

        modelBuilder.Entity<Ticketstatushistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PRIMARY");

            entity.ToTable("ticketstatushistory");

            entity.HasIndex(e => e.ChangedByUserId, "ChangedByUserId");

            entity.HasIndex(e => e.TicketId, "TicketId");

            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NewStatus).HasColumnType("enum('Open','In Progress','Closed')");
            entity.Property(e => e.OldStatus).HasColumnType("enum('Open','In Progress','Closed')");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.Ticketstatushistories)
                .HasForeignKey(d => d.ChangedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticketstatushistory_ibfk_2");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Ticketstatushistories)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("ticketstatushistory_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'User'")
                .HasColumnType("enum('User','Admin')");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
