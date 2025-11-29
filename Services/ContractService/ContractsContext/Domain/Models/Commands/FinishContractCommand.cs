using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record FinishContractCommand(
    Guid ContractId,
    string Reason
) : IRequest<Unit>;
