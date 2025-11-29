namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ContractActivatedEvent(
    Guid ContractId,
    Guid OfficeId,
    DateTime ActivatedAt
);