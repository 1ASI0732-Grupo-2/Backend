namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ContractFinishedEvent(
    Guid ContractId,
    Guid OfficeId,
    DateTime TerminatedAt,
    string Reason
);
