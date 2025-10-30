using System;

namespace workstation_backend.ContractsContext.Domain.Models.Entities;

public class Signature
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ContractId { get; private set; }
    public Guid SignerId { get; private set; }
    public DateTime SignedAt { get; private set; } = DateTime.UtcNow;

    public string SignatureHash { get; private set; } = string.Empty;
    public Signature(Guid contractId, Guid signerId, string signatureHash)
    {
        ContractId = contractId;
        SignerId = signerId;
        SignatureHash = signatureHash;
    }

    private Signature() { }
}
