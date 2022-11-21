using Jobsity.Chat.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chat.Persistence.EntityFramework;

public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseInMemoryDatabase(databaseName: nameof(ChatContext));

    public DbSet<UserChat> UserChats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserChat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.DateCreated).IsRequired();
        });
    }
}