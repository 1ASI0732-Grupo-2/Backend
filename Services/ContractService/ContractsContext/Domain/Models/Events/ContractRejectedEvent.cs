namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ContractRejectedEvent(
    Guid ContractId,
    Guid RejectedBy,
    string Reason,
    DateTime OccurredAt
);