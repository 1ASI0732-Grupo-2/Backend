using System.Security.Policy;
using ContractsContext.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using workstation_backend.ContractsContext.Domain.Models.Entities;

namespace workstation_backend.Shared.Infrastructure.Persistence.Configuration;


public class ContractContext : DbContext
{
    public ContractContext(DbContextOptions<ContractContext> options) : base(options)
    {
    }

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
                    property.SetColumnType("timestamptz");
                }
            }
        }

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
            entity.Property(c => c.StartDate)
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(c => c.EndDate)
                .HasColumnType("timestamptz")
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd()
                .IsRequired();

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

            entity.HasIndex(c => c.OfficeId);
            entity.HasIndex(c => c.OwnerId);
            entity.HasIndex(c => c.RenterId);
            entity.HasIndex(c => c.Status);
        });

        builder.Entity<Clause>(entity =>
        {
            entity.ToTable("Clauses");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).ValueGeneratedOnAdd();
            entity.Property(c => c.ContractId).IsRequired();
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Content).HasMaxLength(2000).IsRequired();
            entity.Property(c => c.Order).IsRequired();
            entity.Property(c => c.Mandatory).IsRequired();

            entity.HasIndex(c => c.ContractId);
        });

        builder.Entity<PaymentReceipt>(entity =>
        {
            entity.ToTable("PaymentReceipts");
            entity.HasKey(pr => pr.Id);

            entity.Property(pr => pr.Id).ValueGeneratedOnAdd();
            entity.Property(pr => pr.ContractId).IsRequired();
            entity.Property(pr => pr.ReceiptNumber).HasMaxLength(50).IsRequired();
            entity.Property(pr => pr.BaseAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(pr => pr.CompensationAdjustments).HasPrecision(18, 2).IsRequired();
            entity.Property(pr => pr.Notes).HasMaxLength(1000);
            entity.Property(pr => pr.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

            entity.Property(pr => pr.IssuedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd()
                .IsRequired();

            entity.Ignore(pr => pr.FinalAmount);

            entity.HasIndex(pr => pr.ContractId).IsUnique();
            entity.HasIndex(pr => pr.ReceiptNumber).IsUnique();
        });

        builder.Entity<Signature>(entity =>
        {
            entity.ToTable("Signatures");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id).ValueGeneratedOnAdd();
            entity.Property(s => s.ContractId).IsRequired();
            entity.Property(s => s.SignerId).IsRequired();
            entity.Property(s => s.SignatureHash).HasMaxLength(256).IsRequired();

            entity.Property(s => s.SignedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd()
                .IsRequired();

            entity.HasIndex(s => s.ContractId);
            entity.HasIndex(s => s.SignerId);
        });

        builder.Entity<Compensation>(entity =>
        {
            entity.ToTable("Compensations");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).ValueGeneratedOnAdd();
            entity.Property(c => c.ContractId).IsRequired();
            entity.Property(c => c.IssuerId).IsRequired();
            entity.Property(c => c.ReceiverId).IsRequired();
            entity.Property(c => c.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(c => c.Reason).HasMaxLength(500).IsRequired();
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(50).IsRequired();

            entity.Property(c => c.CreatedAt)
                .HasColumnType("timestamptz")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd()
                .IsRequired();

            entity.HasIndex(c => c.ContractId);
            entity.HasIndex(c => c.IssuerId);
            entity.HasIndex(c => c.ReceiverId);
        });
    }
}