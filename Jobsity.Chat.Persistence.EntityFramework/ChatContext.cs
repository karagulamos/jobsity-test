using Jobsity.Chat.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chat.Persistence.EntityFramework;

public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }

    public DbSet<UserChat> UserChats { get; set; } = default!;
    public DbSet<ChatRoom> ChatRooms { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<UserChat>(entity =>
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

        _ = modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        _ = modelBuilder.Entity<ChatRoom>().HasData(
            new ChatRoom(Guid.Parse("c0a80101-0000-0000-0000-000000000000"), "General"),
            new ChatRoom(Guid.Parse("c0a80101-0000-0000-0000-000000000001"), "Random"),
            new ChatRoom(Guid.Parse("c0a80101-0000-0000-0000-000000000002"), "Jobsity")
        );
    }
}

// dotnet add Jobsity.Chat.Web package Microsoft.EntityFrameworkCore.Design
// dotnet ef database update -p Jobsity.Chat.Persistence.EntityFramework -s Jobsity.Chat.Web -c ChatContext

// dotnet remove package Microsoft.VisualStudio.Web.CodeGeneration.Design
// dotnet remove package Microsoft.EntityFrameworkCore.Design
// dotnet remove package Microsoft.AspNetCore.Identity.EntityFrameworkCore
// dotnet remove package Microsoft.AspNetCore.Identity.UI
// dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
// dotnet remove package Microsoft.EntityFrameworkCore.Tools

