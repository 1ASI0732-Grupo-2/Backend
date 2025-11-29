namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ReceiptUpdatedEvent(
    Guid ReceiptId,
    Guid ContractId,
    decimal CompensationAdjustments,
    decimal FinalAmount,
    string Notes,
    DateTime UpdatedAt
);
