using Jobsity.Chat.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chat.Persistence.EntityFramework;

public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }

    public DbSet<UserChat> UserChats { get; set; } = null!;
    public DbSet<ChatRoom> ChatRooms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserChat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ChatRoom)
                  .WithMany()
                  .HasForeignKey(e => e.RoomId);
            entity.Property(e => e.RoomId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.DateCreated).IsRequired();
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<ChatRoom>().HasData(
            new ChatRoom(Guid.Parse("c0a80101-0000-0000-0000-000000000000"), "General"),
            new ChatRoom(Guid.Parse("c0a80101-0000-0000-0000-000000000001"), "Random"),
            new ChatRoom(Guid.Parse("c0a80101-0000-0000-0000-000000000002"), "Jobsity")
        );
    }
}