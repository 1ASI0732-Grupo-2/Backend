using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record AddCompensationCommand(
    Guid ContractId,
    Guid IssuerId,
    Guid ReceiverId,
    decimal Amount,
    string Reason
) : IRequest<Guid>;
