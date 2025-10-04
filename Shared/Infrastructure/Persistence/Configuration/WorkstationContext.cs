using Microsoft.EntityFrameworkCore;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Models.Entities;

namespace workstation_backend.Shared.Infrastructure.Persistence.Configuration;

public class WorkstationContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Office> Offices { get; set; }
    public DbSet<OfficeService> Services { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Office
        builder.Entity<Office>(entity =>
        {
            entity.ToTable("Offices");
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Location).IsRequired().HasMaxLength(200);
            entity.HasIndex(o => o.Location).IsUnique();

            entity.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(500);

            entity.Property(o => o.ImageUrl)
            .HasMaxLength(300); 

            entity.Property(o => o.Capacity).IsRequired();
            entity.Property(o => o.CostPerDay).IsRequired();
            entity.Property(o => o.Available).IsRequired();

            entity.Property(o => o.CreatedDate).HasColumnType("DATETIME");
            entity.Property(o => o.ModifiedDate).HasColumnType("DATETIME");

            entity.HasMany(o => o.Services)
                .WithOne(s => s.Office)
                .HasForeignKey(s => s.OfficeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(o => o.Ratings)
                .WithOne(r => r.Office)
                .HasForeignKey(r => r.OfficeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OfficeService
        builder.Entity<OfficeService>(entity =>
        {
            entity.ToTable("OfficeServices");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Description).IsRequired().HasMaxLength(500);
            entity.Property(s => s.Cost).IsRequired();
            entity.Property(s => s.CreatedDate).HasColumnType("DATETIME");
            entity.Property(s => s.ModifiedDate).HasColumnType("DATETIME");

            entity.HasIndex(s => new { s.OfficeId, s.Name }).IsUnique();
        });

        // Rating
        builder.Entity<Rating>(entity =>
        {
            entity.ToTable("Ratings");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Score).IsRequired();
            entity.Property(r => r.Comment).HasMaxLength(500);
            entity.Property(r => r.CreatedDate).HasColumnType("DATETIME");
            entity.Property(r => r.CreatedAt).HasColumnType("DATETIME");
            entity.Property(r => r.ModifiedDate).HasColumnType("DATETIME");
        });

        // User
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Dni).IsRequired().HasMaxLength(20);
            entity.HasIndex(u => u.Dni).IsUnique();

            entity.Property(u => u.PhoneNumber).HasMaxLength(20);
            entity.Property(u => u.Email).HasMaxLength(100);

            entity.Property(u => u.Role).IsRequired().HasConversion<int>();
            entity.Property(u => u.CreatedDate).HasColumnType("DATETIME");
            entity.Property(u => u.CreatedAt).HasColumnType("DATETIME");
            entity.Property(u => u.ModifiedDate).HasColumnType("DATETIME");
        });
    }
}
