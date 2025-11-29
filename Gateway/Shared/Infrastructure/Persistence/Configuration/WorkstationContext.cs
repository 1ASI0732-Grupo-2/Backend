using System.Security.Policy;
using Microsoft.EntityFrameworkCore;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        // Configuración global para DateTime
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("datetime");
                }
            }
        }

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

            // ✅ CORREGIDO: Valores por defecto y nullable
            entity.Property(o => o.CreatedDate)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(o => o.ModifiedDate)
                .HasColumnType("DATETIME")
                .IsRequired(false); // Nullable

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

            // ✅ CORREGIDO: Valores por defecto y nullable
            entity.Property(s => s.CreatedDate)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(s => s.ModifiedDate)
                .HasColumnType("DATETIME")
                .IsRequired(false); // Nullable

            entity.HasIndex(s => new { s.OfficeId, s.Name }).IsUnique();
        });

        // Rating
        builder.Entity<Rating>(entity =>
        {
            entity.ToTable("Ratings");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Score).IsRequired();
            entity.Property(r => r.Comment).HasMaxLength(500);

            // ✅ CORREGIDO: Valores por defecto
            entity.Property(r => r.CreatedDate)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(r => r.CreatedAt)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(r => r.ModifiedDate)
                .HasColumnType("DATETIME")
                .IsRequired(false); // Nullable
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

            // ✅ CORREGIDO: Valores por defecto
            entity.Property(u => u.CreatedDate)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(u => u.CreatedAt)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(u => u.ModifiedDate)
                .HasColumnType("DATETIME")
                .IsRequired(false); // Nullable
        });
    }
}