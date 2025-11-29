using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record ActivateContractCommand(Guid ContractId) : IRequest<Unit>;
