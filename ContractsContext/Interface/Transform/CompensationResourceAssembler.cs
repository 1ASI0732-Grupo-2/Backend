using System;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class CompensationResourceAssembler
{
public static CompensationResource ToResource(Compensation compensation)
    {
        return new CompensationResource(
            compensation.Id,
            compensation.IssuerId,
            compensation.ReceiverId,
            compensation.Amount,
            compensation.Reason,
            compensation.CreatedAt,
            compensation.Status.ToString()
        );
    }
}
