using System.Security.Policy;
using ContractsContext.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using workstation_backend.ContractsContext.Domain.Models.Entities;
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
    public DbSet<Clause> Clauses { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
    public DbSet<Signature> Signatures { get; set; }
    public DbSet<Compensation> Compensations { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

        // Contract
        builder.Entity<Contract>(entity =>
        {
            entity.ToTable("Contracts");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.OfficeId).IsRequired();
            entity.Property(c => c.OwnerId).IsRequired();
            entity.Property(c => c.RenterId).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(500).IsRequired();
            entity.Property(c => c.BaseAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(c => c.LateFee).HasPrecision(18, 2).IsRequired();
            entity.Property(c => c.InterestRate).HasPrecision(5, 2).IsRequired();
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(c => c.StartDate).HasColumnType("DATETIME").IsRequired();
            entity.Property(c => c.EndDate).HasColumnType("DATETIME").IsRequired();
            entity.Property(c => c.CreatedAt).HasColumnType("DATETIME").IsRequired();
            entity.HasMany(c => c.Clauses)
                .WithOne()
                .HasForeignKey(cl => cl.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Signatures)
                .WithOne()
                .HasForeignKey(s => s.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Compensations)
                .WithOne()
                .HasForeignKey(co => co.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Receipt)
                .WithOne()
                .HasForeignKey<PaymentReceipt>(pr => pr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            entity.HasIndex(c => c.OfficeId);
            entity.HasIndex(c => c.OwnerId);
            entity.HasIndex(c => c.RenterId);
            entity.HasIndex(c => c.Status);
        });

        builder.Entity<Clause>(entity =>
        {
            entity.ToTable("Clauses");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.ContractId)
                .IsRequired();

            entity.Property(c => c.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(c => c.Content)
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(c => c.Order)
                .IsRequired();

            entity.Property(c => c.Mandatory)
                .IsRequired();

            entity.HasIndex(c => c.ContractId);
        });


        builder.Entity<PaymentReceipt>(entity =>
        {
            entity.ToTable("PaymentReceipts");

            entity.HasKey(pr => pr.Id);

            entity.Property(pr => pr.Id)
                .ValueGeneratedOnAdd();

            entity.Property(pr => pr.ContractId)
                .IsRequired();

            entity.Property(pr => pr.ReceiptNumber)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(pr => pr.BaseAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(pr => pr.CompensationAdjustments)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(pr => pr.Notes)
                .HasMaxLength(1000);

            entity.Property(pr => pr.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(pr => pr.IssuedAt)
                .IsRequired();

            entity.Ignore(pr => pr.FinalAmount);

            entity.HasIndex(pr => pr.ContractId)
                .IsUnique();

            entity.HasIndex(pr => pr.ReceiptNumber)
                .IsUnique();

        });

        builder.Entity<Signature>(entity =>
        {

            entity.ToTable("Signatures");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();
            entity.Property(s => s.ContractId)
                .IsRequired();

            entity.Property(s => s.SignerId)
                .IsRequired();

            entity.Property(s => s.SignatureHash)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(s => s.SignedAt)
                .IsRequired();

            entity.HasIndex(s => s.ContractId);
            entity.HasIndex(s => s.SignerId);
        });

        builder.Entity<Compensation>(entity =>
        {
            entity.ToTable("Compensations");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.ContractId)
                .IsRequired();

            entity.Property(c => c.IssuerId)
                .IsRequired();

            entity.Property(c => c.ReceiverId)
                .IsRequired();

            entity.Property(c => c.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(c => c.Reason)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(c => c.CreatedAt).HasColumnType("DATETIME")
                .IsRequired();

            entity.HasIndex(c => c.ContractId);
            entity.HasIndex(c => c.IssuerId);
            entity.HasIndex(c => c.ReceiverId);
        });


    }
}
