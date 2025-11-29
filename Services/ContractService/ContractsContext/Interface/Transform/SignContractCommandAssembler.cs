using System;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class SignContractCommandAssembler
{
public static SignContractCommand ToCommand(Guid contractId, SignContractResource resource)
    {
        return new SignContractCommand(
            contractId,
            resource.SignerId,
            resource.SignatureHash
        );
    }
}
