using System;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Domain.Models.Entities;

public class PaymentReceipt
{
    public Guid Id { get; private set; } 
    public Guid ContractId { get; private set; }
    public string ReceiptNumber { get; private set; } = string.Empty;
    public decimal BaseAmount { get; private set; }
    public decimal CompensationAdjustments { get; private set; }
    public decimal FinalAmount => BaseAmount + CompensationAdjustments;

    public DateTime IssuedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public PaymentReceiptStatus Status { get; private set; } = PaymentReceiptStatus.Issued;

    public PaymentReceipt(Guid contractId, string receiptNumber, decimal baseAmount)
    {
        ContractId = contractId;
        ReceiptNumber = receiptNumber;
        BaseAmount = baseAmount;
    }
private PaymentReceipt() { }

    public void UpdateWithCompensations(decimal adjustment, string note)
    {
        CompensationAdjustments = adjustment;
        Status = PaymentReceiptStatus.Updated;
        UpdatedAt = DateTime.UtcNow;
        Notes = note;
    }

    public void FinalizeReceipt()
    {
        Status = PaymentReceiptStatus.Finalized;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = PaymentReceiptStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
