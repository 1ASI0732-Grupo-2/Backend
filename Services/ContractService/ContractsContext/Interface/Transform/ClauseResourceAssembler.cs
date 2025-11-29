using System;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class ClauseResourceAssembler
{
public static ClauseResource ToResource(Clause clause)
    {
        return new ClauseResource(
            clause.Id,
            clause.Name,
            clause.Content,
            clause.Order,
            clause.Mandatory
        );
    }
}
