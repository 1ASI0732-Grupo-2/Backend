using System;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Domain.Models.Entities;

public class Compensation
{
    public Guid Id { get; private set; }
    public Guid ContractId { get; private set; }
    public Guid IssuerId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public CompensationStatus Status { get; private set; } = CompensationStatus.Pending;

    public Compensation(Guid contractId, Guid issuerId, Guid receiverId, decimal amount, string reason)
    {
        ContractId = contractId;
        IssuerId = issuerId;
        ReceiverId = receiverId;
        Amount = amount;
        Reason = reason;
    }

    private Compensation() { }

    public void Approve() => Status = CompensationStatus.Approved;
    public void Reject() => Status = CompensationStatus.Rejected;
}
