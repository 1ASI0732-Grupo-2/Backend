using System;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class PaymentReceiptResourceAssembler
{
public static PaymentReceiptResource ToResource(PaymentReceipt receipt)
    {
        return new PaymentReceiptResource(
            receipt.Id,
            receipt.ReceiptNumber,
            receipt.BaseAmount,
            receipt.CompensationAdjustments,
            receipt.FinalAmount,
            receipt.IssuedAt,
            receipt.UpdatedAt,
            receipt.Notes,
            receipt.Status.ToString()
        );
    }
}
