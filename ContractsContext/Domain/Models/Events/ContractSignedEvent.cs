namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ContractSignedEvent(
    Guid ContractId,
    Guid SignerId,
    string SignatureHash,
    DateTime SignedAt,
    bool AllPartiesSigned
);
