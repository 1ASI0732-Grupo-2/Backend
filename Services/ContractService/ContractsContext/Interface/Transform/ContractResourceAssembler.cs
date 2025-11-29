using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class ContractResourceAssembler
{
    public static ContractResource ToResource(Contract? contract)
    {
        if (contract == null) return null!;

        return new ContractResource(
            contract.Id,
            contract.OfficeId,
            contract.OwnerId,
            contract.RenterId,
            contract.Description,
            contract.StartDate,
            contract.EndDate,
            contract.BaseAmount,
            contract.LateFee,
            contract.InterestRate,
            contract.Status.ToString(),
            contract.CreatedAt,
            contract.ActivatedAt,
            contract.TerminatedAt,
            contract.Clauses.Select(ClauseResourceAssembler.ToResource).ToList(),
            contract.Signatures.Select(SignatureResourceAssembler.ToResource).ToList(),
            contract.Compensations.Select(CompensationResourceAssembler.ToResource).ToList(),
            contract.Receipt != null ? PaymentReceiptResourceAssembler.ToResource(contract.Receipt) : null
        );
    }
}
