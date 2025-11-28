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

    public ICollection<Clause> Clauses { get; } = new List<Clause>();
    public ICollection<Signature> Signatures { get; } = new List<Signature>();
    public ICollection<Compensation> Compensations { get; } = new List<Compensation>();

    public PaymentReceipt? Receipt { get; private set; }

    /// <summary>
    /// Constructor para crear un nuevo contrato
    /// </summary>
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

    /// <summary>
    /// Constructor sin parámetros para Entity Framework
    /// </summary>
    public Contract() 
    {
    }

    public void AddClause(Clause clause)
    {
        if (clause == null)
            throw new ArgumentNullException(nameof(clause));
            
        if (Status != ContractStatus.Draft && Status != ContractStatus.PendingSignatures)
            throw new InvalidOperationException("No se pueden agregar cláusulas después de que el contrato esté activo.");
        
        Clauses.Add(clause);
    }

    public void AddSignature(Signature signature)
    {
        if (signature == null)
            throw new ArgumentNullException(nameof(signature));
            
        if (Status == ContractStatus.Active)
            throw new InvalidOperationException("El contrato ya está activo.");
        
        Signatures.Add(signature);
        
        var ownerSigned = Signatures.Any(s => s.SignerId == OwnerId);
        var renterSigned = Signatures.Any(s => s.SignerId == RenterId);
        
        if (ownerSigned && renterSigned && Status == ContractStatus.Draft)
        {
            Status = ContractStatus.PendingSignatures;
        }
    }

    public void AddCompensation(Compensation compensation)
    {
        if (Status != ContractStatus.Active)
            throw new InvalidOperationException("Las compensaciones solo se permiten en contratos activos.");

        Compensations.Add(compensation);
    }

    public void SetReceipt(PaymentReceipt receipt)
    {
        Receipt = receipt;
    }

    public void Activate()
    {
        if (Status != ContractStatus.PendingSignatures)
            throw new InvalidOperationException("El contrato debe estar pendiente de firmas.");

        var ownerSigned = Signatures.Any(s => s.SignerId == OwnerId);
        var renterSigned = Signatures.Any(s => s.SignerId == RenterId);

        if (!ownerSigned || !renterSigned)
            throw new InvalidOperationException("Ambas firmas son necesarias para activar el contrato.");

        Status = ContractStatus.Active;
        ActivatedAt = DateTime.UtcNow;
    }

    public void Terminate()
    {
        var hasPendingCompensations = Compensations.Any(c => c.Status == CompensationStatus.Pending);
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