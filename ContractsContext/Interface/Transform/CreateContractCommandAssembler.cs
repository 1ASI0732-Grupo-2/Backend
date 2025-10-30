using System;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public class CreateContractCommandAssembler
{
    public static CreateContractCommand ToCommand(CreateContractResource resource)
    {
        return new CreateContractCommand(
            resource.OfficeId,
            resource.OwnerId,
            resource.RenterId,
            resource.Description,
            resource.StartDate,
            resource.EndDate,
            resource.BaseAmount,
            resource.LateFee,
            resource.InterestRate
        );
    }

}
