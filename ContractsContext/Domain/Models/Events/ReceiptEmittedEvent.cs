namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ReceiptEmittedEvent(
    Guid ContractId,
    Guid ReceiptId,
    string ReceiptNumber,
    decimal BaseAmount,
    DateTime IssuedAt
);
