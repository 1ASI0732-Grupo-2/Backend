namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ClauseAddedEvent(
    Guid ContractId,
    Guid ClauseId,
    string ClauseName,
    bool IsMandatory,
    DateTime OccurredAt
);
