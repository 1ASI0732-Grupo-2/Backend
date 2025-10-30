using System;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class UpdateReceiptCommandAssembler
{
public static UpdateReceiptCommand ToCommand(Guid contractId, UpdateReceiptResource resource)
    {
        return new UpdateReceiptCommand(
            contractId,
            resource.CompensationAdjustments,
            resource.Notes
        );
    }
}
