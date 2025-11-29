namespace workstation_backend.ContractsContext.Interface.Resources;

public record class CompensationResource(
    Guid Id,
    Guid IssuerId,
    Guid ReceiverId,
    decimal Amount,
    string Reason,
    DateTime CreatedAt,
    string Status
);
