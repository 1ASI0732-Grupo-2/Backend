using ContractsContext.Domain.Models.Entities;
using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Queries;

public record GetContractByIdQuery(Guid ContractId) : IRequest<Contract?>;