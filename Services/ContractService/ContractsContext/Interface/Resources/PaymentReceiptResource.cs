namespace workstation_backend.ContractsContext.Interface.Resources;

public record class PaymentReceiptResource(    Guid Id,
    string ReceiptNumber,
    decimal BaseAmount,
    decimal CompensationAdjustments,
    decimal FinalAmount,
    DateTime IssuedAt,
    DateTime? UpdatedAt,
    string Notes,
    string Status);

