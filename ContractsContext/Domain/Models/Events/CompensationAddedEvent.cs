namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record CompensationAddedEvent(
    Guid ContractId,
    Guid CompensationId,
    Guid IssuerId,
    Guid ReceiverId,
    decimal Amount,
    string Reason,
    DateTime OccurredAt
);
