using System;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class AddClauseCommandAssembler
{
 public static AddClauseCommand ToCommand(Guid contractId, AddClauseResource resource)
    {
        return new AddClauseCommand(
            contractId,
            resource.Name,
            resource.Content,
            resource.Order,
            resource.Mandatory
        );
    }
}
