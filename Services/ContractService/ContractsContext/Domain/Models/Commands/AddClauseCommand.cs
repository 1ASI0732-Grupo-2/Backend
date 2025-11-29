using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record AddClauseCommand(
    Guid ContractId,
    string Name,
    string Content,
    int Order,
    bool Mandatory
) : IRequest<Guid>;
