using System;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class AddCompensationCommandAssembler
{
     public static AddCompensationCommand ToCommand(Guid contractId, AddCompensationResource resource)
    {
        return new AddCompensationCommand(
            contractId,
            resource.IssuerId,
            resource.ReceiverId,
            resource.Amount,
            resource.Reason
        );
    }
}
