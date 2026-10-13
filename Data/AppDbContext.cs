using Microsoft.EntityFrameworkCore;
using Notes.Api.Models;

namespace Notes.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.CreatedAtUtc)
                .IsRequired();

            entity.Property(u => u.LastLoginAtUtc)
                .IsRequired(false);
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.ToTable("Notes");

            entity.HasKey(n => n.Id);

            entity.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(n => n.Content)
                .IsRequired();

            entity.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(n => n.Color)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(n => n.CreatedAtUtc)
                .IsRequired();

            entity.Property(n => n.UpdatedAtUtc)
                .IsRequired(false);

            entity.Property(n => n.TargetDateUtc)
                .IsRequired(false);

            entity.HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(n => n.UserId);
            entity.HasIndex(n => new { n.UserId, n.CreatedAtUtc });
        });
    }
}
