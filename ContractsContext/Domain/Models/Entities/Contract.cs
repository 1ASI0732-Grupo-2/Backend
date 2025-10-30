
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace ContractsContext.Domain.Models.Entities;
public class Contract
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OfficeId { get; private set; }
    public Guid OwnerId { get; private set; }
    public Guid RenterId { get; private set; }

    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public decimal BaseAmount { get; private set; }
    public decimal LateFee { get; private set; }
    public decimal InterestRate { get; private set; }

    public ContractStatus Status { get; private set; } = ContractStatus.Draft;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? TerminatedAt { get; private set; }

    private readonly List<Clause> _clauses = new();
    public IReadOnlyCollection<Clause> Clauses => _clauses.AsReadOnly();

    private readonly List<Signature> _signatures = new();
    public IReadOnlyCollection<Signature> Signatures => _signatures.AsReadOnly();

    private readonly List<Compensation> _compensations = new();
    public IReadOnlyCollection<Compensation> Compensations => _compensations.AsReadOnly();

    public PaymentReceipt? Receipt { get; private set; }

    public Contract(Guid officeId, Guid ownerId, Guid renterId, string description, DateTime startDate, DateTime endDate, decimal baseAmount, decimal lateFee, decimal interestRate)
    {
        OfficeId = officeId;
        OwnerId = ownerId;
        RenterId = renterId;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        BaseAmount = baseAmount;
        LateFee = lateFee;
        InterestRate = interestRate;
    }

    public Contract() { }

    public void AddClause(Clause clause)
    {
        if (Status != ContractStatus.Draft && Status != ContractStatus.PendingSignatures)
            throw new InvalidOperationException("No se pueden agregar cláusulas después de que el contrato esté activo.");

        _clauses.Add(clause);
    }

    public void AddSignature(Signature signature)
    {
        if (Status == ContractStatus.Active)
            throw new InvalidOperationException("El contrato ya está activo.");

        _signatures.Add(signature);

        // Verificar si ambas partes han firmado
        var ownerSigned = _signatures.Any(s => s.SignerId == OwnerId);
        var renterSigned = _signatures.Any(s => s.SignerId == RenterId);

        if (ownerSigned && renterSigned && Status == ContractStatus.Draft)
        {
            Status = ContractStatus.PendingSignatures;
        }
    }

    public void AddCompensation(Compensation compensation)
    {
        if (Status != ContractStatus.Active)
            throw new InvalidOperationException("Las compensaciones solo se permiten en contratos activos.");

        _compensations.Add(compensation);
    }

    public void SetReceipt(PaymentReceipt receipt)
    {
        Receipt = receipt;
    }

    public void Activate()
    {
        if (Status != ContractStatus.PendingSignatures)
            throw new InvalidOperationException("El contrato debe estar pendiente de firmas.");

        var ownerSigned = _signatures.Any(s => s.SignerId == OwnerId);
        var renterSigned = _signatures.Any(s => s.SignerId == RenterId);

        if (!ownerSigned || !renterSigned)
            throw new InvalidOperationException("Ambas firmas son necesarias para activar el contrato.");

        Status = ContractStatus.Active;
        ActivatedAt = DateTime.UtcNow;
    }

    public void Terminate()
    {
        var hasPendingCompensations = _compensations.Any(c => c.Status == CompensationStatus.Pending);
        if (hasPendingCompensations)
            throw new InvalidOperationException("El contrato no puede finalizar con compensaciones pendientes.");

        Status = ContractStatus.Completed;
        TerminatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ContractStatus.Active)
            throw new InvalidOperationException("No se puede cancelar un contrato activo. Use Terminate en su lugar.");

        Status = ContractStatus.Cancelled;
        TerminatedAt = DateTime.UtcNow;
    }
}

