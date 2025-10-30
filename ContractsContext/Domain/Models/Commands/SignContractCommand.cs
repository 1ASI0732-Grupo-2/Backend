using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record SignContractCommand(
    Guid ContractId,
    Guid SignerId,
    string SignatureHash
) : IRequest<Guid>;
